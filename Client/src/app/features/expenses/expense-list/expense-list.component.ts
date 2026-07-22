import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { getCategoryMeta, ALL_CATEGORIES } from '../../../core/utils/category.utils';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { ShareTablePromptComponent } from '../../expense-table/share-table-prompt.component';
import { Expense, ExpenseCategory } from '../../../core/models/expense.model';

type SortColumn = 'date' | 'description' | 'amount' | 'category' | 'merchant';
type SortDirection = 'asc' | 'desc';

interface ExpenseGroup {
  key: string;
  expenses: Expense[];
}

interface ExpenseRow {
  expense: Expense;
  isGroupStart: boolean;
  groupSize: number;
  groupKey: string;
}

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [DecimalPipe, DatePipe, FormsModule, ConfirmDialogComponent, ShareTablePromptComponent],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseListComponent implements OnInit {
  readonly store  = inject(ExpenseService);
  readonly expenseTableService = inject(ExpenseTableService);
  readonly router = inject(Router);
  readonly route  = inject(ActivatedRoute);

  readonly tableId = signal<number>(0);
  readonly isStarred = computed(() => this.expenseTableService.tables().find(t => t.id === this.tableId())?.isStarred ?? false);
  readonly pageTitle = computed(() => this.expenseTableService.tables().find(t => t.id === this.tableId())?.name ?? 'Expenses');
  readonly isAdmin = computed(() => this.expenseTableService.tables().find(t => t.id === this.tableId())?.isCurrentUserAdmin ?? false);
  readonly showShareDialog = signal(false);

  getCategoryMeta = getCategoryMeta;
  readonly categories = ALL_CATEGORIES;

  readonly deletingId  = signal<number | null>(null);
  readonly actionError = signal<string | null>(null);
  readonly hoveredGroupKey = signal<string | null>(null);

  readonly sortColumn    = signal<SortColumn>('date');
  readonly sortDirection = signal<SortDirection>('desc');

  private readonly queryParams = toSignal(this.route.queryParamMap);

  readonly selectedMonth    = computed(() => this.queryParams()?.get('month') ?? null);
  readonly selectedCategory = computed(() => this.queryParams()?.get('category') as ExpenseCategory | null ?? null);

  readonly availableMonths = computed(() => {
    const now = new Date();
    const year = now.getFullYear();
    const currentMonth = now.getMonth();

    const months: { key: string; label: string }[] = [];
    for (let m = currentMonth; m >= 0; m--) {
      const key = `${year}-${String(m + 1).padStart(2, '0')}`;
      months.push({ key, label: this.formatMonthLabel(key) });
    }
    return months;
  });

  readonly filteredExpenses = computed(() => {
    let list = this.store.expenses();

    const month = this.selectedMonth();
    if (month) list = list.filter(e => e.date.slice(0, 7) === month);

    const category = this.selectedCategory();
    if (category) list = list.filter(e => e.category === category);

    return list;
  });

  readonly filteredTotal = computed(() =>
    this.filteredExpenses().reduce((sum, e) => sum + e.amount, 0)
  );

  readonly filteredCount = computed(() => this.filteredExpenses().length);

  private compareExpenses(a: Expense, b: Expense): number {
    const column = this.sortColumn();
    const direction = this.sortDirection() === 'asc' ? 1 : -1;
    switch (column) {
      case 'date':        return (new Date(a.date).getTime() - new Date(b.date).getTime()) * direction;
      case 'amount':      return (a.amount - b.amount) * direction;
      case 'description': return a.description.localeCompare(b.description) * direction;
      case 'category':    return a.category.localeCompare(b.category) * direction;
      case 'merchant':    return (a.merchant ?? '').localeCompare(b.merchant ?? '') * direction;
    }
  }

  readonly groupedExpenses = computed<ExpenseGroup[]>(() => {
    const expenses = this.filteredExpenses();
    const buckets = new Map<string, Expense[]>();

    for (const e of expenses) {
      const key = e.receiptId != null ? `r${e.receiptId}` : `e${e.id}`;
      const bucket = buckets.get(key);
      if (bucket) bucket.push(e); else buckets.set(key, [e]);
    }

    const groups: ExpenseGroup[] = Array.from(buckets.entries()).map(([key, groupExpenses]) => ({
      key,
      expenses: [...groupExpenses].sort((a, b) => this.compareExpenses(a, b)),
    }));

    groups.sort((a, b) => this.compareExpenses(a.expenses[0], b.expenses[0]));

    return groups;
  });

  private readonly PAGE_SIZE = 10;
  readonly currentPage = signal(1);

  readonly pages = computed<ExpenseRow[][]>(() => {
    const groups = this.groupedExpenses();
    const pages: ExpenseRow[][] = [];
    let current: ExpenseRow[] = [];

    for (const group of groups) {
      const rows: ExpenseRow[] = group.expenses.map((expense, idx) => ({
        expense, isGroupStart: idx === 0, groupSize: group.expenses.length, groupKey: group.key,
      }));

      if (current.length > 0 && current.length + rows.length > this.PAGE_SIZE) {
        pages.push(current);
        current = [];
      }
      current.push(...rows);
    }
    if (current.length > 0) pages.push(current);
    return pages.length > 0 ? pages : [[]];
  });

  readonly totalPages = computed(() => this.pages().length);
  readonly safeCurrentPage = computed(() => Math.min(this.currentPage(), this.totalPages()));
  readonly pagedRows = computed<ExpenseRow[]>(() => this.pages()[this.safeCurrentPage() - 1] ?? []);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const tableId = Number(params.get('tableId'));
      this.tableId.set(tableId);
      this.store.loadAll(tableId);
    });
  }

  formatMonthLabel(monthKey: string): string {
    const [year, month] = monthKey.split('-').map(Number);
    return new Date(year, month - 1, 1).toLocaleDateString('en-GB', { month: 'long', year: 'numeric' });
  }

  onMonthChange(value: string): void {
    this.currentPage.set(1);
    this.updateQueryParams({ month: value || null });
  }

  onCategoryChange(value: string): void {
    this.currentPage.set(1);
    this.updateQueryParams({ category: value || null });
  }

  resetFilters(): void {
    this.currentPage.set(1);
    this.updateQueryParams({ month: null, category: null });
  }

  private updateQueryParams(params: Record<string, string | null>): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: params,
      queryParamsHandling: 'merge',
    });
  }

  toggleSort(column: SortColumn): void {
    this.currentPage.set(1);
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
  }

  sortIcon(column: SortColumn): string {
    if (this.sortColumn() !== column) return '▲';
    return this.sortDirection() === 'asc' ? '▲' : '▼';
  }

  goToPage(page: number): void {
    this.currentPage.set(Math.min(Math.max(1, page), this.totalPages()));
  }

  prevPage(): void {
    this.goToPage(this.safeCurrentPage() - 1);
  }

  nextPage(): void {
    this.goToPage(this.safeCurrentPage() + 1);
  }

  viewExpense(id: number): void {
    this.router.navigate(['/expenses/table', this.tableId(), id]);
  }

  onMerchantCellClick(event: MouseEvent, expense: Expense): void {
    if (expense.receiptId != null) {
      event.stopPropagation();
      this.router.navigate(['/expenses/table', this.tableId(), 'receipt', expense.receiptId]);
    }
  }

  editExpense(id: number): void {
    this.router.navigate(['/expenses/table', this.tableId(), id, 'edit']);
  }

  confirmDelete(id: number): void {
    this.actionError.set(null);
    this.store.deleteExpense(
      this.tableId(),
      id,
      () => this.deletingId.set(null),
      (msg) => {
        this.deletingId.set(null);
        this.actionError.set(msg);
      }
    );
  }

  toggleStar(): void {
    const id = this.tableId();
    if (this.isStarred()) {
      this.expenseTableService.unstarTable(id, () => {}, () => {});
    } else {
      this.expenseTableService.starTable(id, () => {}, () => {});
    }
  }
}
