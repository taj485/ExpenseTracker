import { ChangeDetectionStrategy, Component, computed, effect, inject, signal } from '@angular/core';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { SummaryCardsComponent } from './components/summary-cards/summary-cards.component';
import { CategoryBreakdownComponent } from './components/category-breakdown/category-breakdown.component';
import { MerchantDonutComponent } from './components/merchant-donut/merchant-donut.component';
import { formatMonthKey } from '../../core/utils/date.utils';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [SummaryCardsComponent, CategoryBreakdownComponent, MerchantDonutComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardComponent {
  readonly pageTitle = 'Dashboard';

  readonly store = inject(ExpenseService);
  private readonly expenseTableService = inject(ExpenseTableService);

  readonly currentTableId = signal<number | null>(null);

  readonly currentTableName = computed(() =>
    this.expenseTableService.tables().find(t => t.id === this.currentTableId())?.name ?? ''
  );

  readonly selectedMonthLabel = computed(() => formatMonthKey(this.store.selectedMonth()));

  onMonthChange(monthKey: string): void {
    this.store.selectedMonth.set(monthKey);
  }

  constructor() {
    effect(() => {
      if (!this.expenseTableService.loaded()) return;

      const tables = this.expenseTableService.tables();
      const target = tables.find(t => t.isStarred) ?? tables[0];
      if (target) {
        this.currentTableId.set(target.id);
        this.store.loadAll(target.id);
      }
    });
  }
}
