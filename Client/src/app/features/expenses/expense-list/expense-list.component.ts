import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { getCategoryMeta } from '../../../core/utils/category.utils';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { MerchantLogoComponent } from '../../../shared/merchant-logo/merchant-logo.component';
import { MembersDialogComponent } from '../../expense-table/members-dialog.component';
import { ShareTablePromptComponent } from '../../expense-table/share-table-prompt.component';
import { Expense, ExpenseCategory } from '../../../core/models/expense.model';
import { expenseTotal } from '../../../core/utils/expense.utils';
import { orderCategoryChips, parseCategoryParam, toggleCategory } from '../../../core/utils/expense-filter.utils';
import { uploaderLabel } from '../../../core/utils/uploader.utils';

type SortColumn = 'date' | 'description' | 'unitPrice' | 'quantity' | 'category' | 'merchant';
type SortDirection = 'asc' | 'desc';

interface ExpenseGroup {
  key: string;
  expenses: Expense[];
}

/** One receipt card: a merchant header carrying the receipt total, over its line items. */
interface ReceiptCard {
  key: string;
  merchant: string | null;
  merchantWebsite: string | null;
  date: string;
  receiptId: number | null;
  total: number;
  /** "you" / "emma.carter", or null when the uploader wasn't recorded. */
  uploader: string | null;
  uploadedByCurrentUser: boolean;
  expenses: Expense[];
}

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [DecimalPipe, DatePipe, FormsModule, ConfirmDialogComponent, ShareTablePromptComponent, MembersDialogComponent, MerchantLogoComponent],
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
  readonly isOnlyTable = computed(() => this.expenseTableService.tables().length === 1);
  readonly memberCount = computed(() => this.expenseTableService.tables().find(t => t.id === this.tableId())?.memberCount ?? 1);
  /** "Added by" is only worth showing when someone else could have added the expense. */
  readonly isSharedTable = computed(() => this.memberCount() > 1);
  readonly showShareDialog = signal(false);
  readonly showMembersDialog = signal(false);
  readonly showSettingsMenu = signal(false);
  readonly confirmingDeleteTable = signal(false);

  getCategoryMeta = getCategoryMeta;

  readonly deletingId  = signal<number | null>(null);
  readonly actionError = signal<string | null>(null);

  // Cards drop the sortable column headers, so ordering is fixed at newest-first.
  // Kept as signals so a sort control can be reintroduced without reworking
  // compareExpenses / groupedExpenses.
  readonly sortColumn    = signal<SortColumn>('date');
  readonly sortDirection = signal<SortDirection>('desc');

  private readonly queryParams = toSignal(this.route.queryParamMap);

  readonly selectedMonth = computed(() => this.queryParams()?.get('month') ?? null);
  /** One or more categories (?category=Food,Health), most recently selected first. */
  readonly selectedCategories = computed(() => parseCategoryParam(this.queryParams()?.get('category') ?? null));
  readonly categoryChips = computed(() => orderCategoryChips(this.selectedCategories()));

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

    const categories = this.selectedCategories();
    if (categories.length > 0) list = list.filter(e => categories.includes(e.category));

    return list;
  });

  readonly filteredTotal = computed(() =>
    this.filteredExpenses().reduce((sum, e) => sum + expenseTotal(e), 0)
  );

  readonly filteredCount = computed(() => this.filteredExpenses().length);

  private compareExpenses(a: Expense, b: Expense): number {
    const column = this.sortColumn();
    const direction = this.sortDirection() === 'asc' ? 1 : -1;
    switch (column) {
      case 'date':        return (new Date(a.date).getTime() - new Date(b.date).getTime()) * direction;
      case 'unitPrice':   return (a.unitPrice - b.unitPrice) * direction;
      case 'quantity':    return (a.quantity - b.quantity) * direction;
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

  /** Header data for each group, so the template does not compute totals inline. */
  readonly receiptCards = computed<ReceiptCard[]>(() =>
    this.groupedExpenses().map((group) => {
      const first = group.expenses[0];
      return {
        key: group.key,
        merchant: first.merchant ?? null,
        merchantWebsite: first.merchantWebsite ?? null,
        date: first.date,
        receiptId: first.receiptId ?? null,
        total: group.expenses.reduce((sum, e) => sum + expenseTotal(e), 0),
        // Every line of a receipt is added together, so the first line's uploader covers the card.
        uploader: uploaderLabel(first, 'short'),
        uploadedByCurrentUser: first.createdByCurrentUser === true,
        expenses: group.expenses,
      };
    })
  );

  // Cards are far taller than table rows, so page by receipt rather than by line.
  private readonly PAGE_SIZE = 25;
  readonly currentPage = signal(1);

  readonly pages = computed<ReceiptCard[][]>(() => {
    const cards = this.receiptCards();
    const pages: ReceiptCard[][] = [];

    for (let i = 0; i < cards.length; i += this.PAGE_SIZE) {
      pages.push(cards.slice(i, i + this.PAGE_SIZE));
    }
    return pages.length > 0 ? pages : [[]];
  });

  readonly totalPages = computed(() => this.pages().length);
  readonly safeCurrentPage = computed(() => Math.min(this.currentPage(), this.totalPages()));
  readonly pagedReceipts = computed<ReceiptCard[]>(() => this.pages()[this.safeCurrentPage() - 1] ?? []);

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

  toggleCategoryFilter(category: ExpenseCategory): void {
    this.currentPage.set(1);
    const next = toggleCategory(this.selectedCategories(), category);
    this.updateQueryParams({ category: next.length > 0 ? next.join(',') : null });
  }

  clearCategories(): void {
    this.currentPage.set(1);
    this.updateQueryParams({ category: null });
  }

  openShareFromMembers(): void {
    this.showMembersDialog.set(false);
    this.showShareDialog.set(true);
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

  viewReceipt(receiptId: number): void {
    this.router.navigate(['/expenses/table', this.tableId(), 'receipt', receiptId]);
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
    if (this.isOnlyTable()) return;

    const id = this.tableId();
    if (this.isStarred()) {
      this.expenseTableService.unstarTable(id, () => {}, () => {});
    } else {
      this.expenseTableService.starTable(id, () => {}, () => {});
    }
  }

  confirmDeleteTable(): void {
    this.actionError.set(null);
    this.expenseTableService.deleteTable(
      this.tableId(),
      () => {
        this.confirmingDeleteTable.set(false);
        this.router.navigate(['/dashboard']);
      },
      (msg) => {
        this.confirmingDeleteTable.set(false);
        this.actionError.set(msg);
      }
    );
  }
}
