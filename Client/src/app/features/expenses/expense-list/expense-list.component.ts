import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../core/services/expense.service';
import { getCategoryMeta, ALL_CATEGORIES } from '../../../core/utils/category.utils';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { ExpenseCategory } from '../../../core/models/expense.model';

type SortColumn = 'date' | 'description' | 'amount' | 'category';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [DecimalPipe, DatePipe, FormsModule, ConfirmDialogComponent],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseListComponent implements OnInit {
  readonly pageTitle = 'All Expenses';

  readonly store  = inject(ExpenseService);
  readonly router = inject(Router);
  readonly route  = inject(ActivatedRoute);

  getCategoryMeta = getCategoryMeta;
  readonly categories = ALL_CATEGORIES;

  readonly deletingId  = signal<number | null>(null);
  readonly actionError = signal<string | null>(null);

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

  readonly sortedExpenses = computed(() => {
    const column = this.sortColumn();
    const expenses = this.filteredExpenses();
    const direction = this.sortDirection() === 'asc' ? 1 : -1;
    return [...expenses].sort((a, b) => {
      switch (column) {
        case 'date':        return (new Date(a.date).getTime() - new Date(b.date).getTime()) * direction;
        case 'amount':      return (a.amount - b.amount) * direction;
        case 'description': return a.description.localeCompare(b.description) * direction;
        case 'category':    return a.category.localeCompare(b.category) * direction;
      }
    });
  });

  ngOnInit(): void {
    this.store.loadAll();
  }

  formatMonthLabel(monthKey: string): string {
    const [year, month] = monthKey.split('-').map(Number);
    return new Date(year, month - 1, 1).toLocaleDateString('en-GB', { month: 'long', year: 'numeric' });
  }

  onMonthChange(value: string): void {
    this.updateQueryParams({ month: value || null });
  }

  onCategoryChange(value: string): void {
    this.updateQueryParams({ category: value || null });
  }

  resetFilters(): void {
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

  viewExpense(id: number): void {
    this.router.navigate(['/expenses', id]);
  }

  editExpense(id: number): void {
    this.router.navigate(['/expenses', id, 'edit']);
  }

  confirmDelete(id: number): void {
    this.actionError.set(null);
    this.store.deleteExpense(
      id,
      () => this.deletingId.set(null),
      (msg) => {
        this.deletingId.set(null);
        this.actionError.set(msg);
      }
    );
  }
}
