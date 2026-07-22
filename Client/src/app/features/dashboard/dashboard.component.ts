import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { SummaryCardsComponent } from './components/summary-cards/summary-cards.component';
import { CategoryBreakdownComponent } from './components/category-breakdown/category-breakdown.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [SummaryCardsComponent, CategoryBreakdownComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardComponent implements OnInit {
  readonly pageTitle = 'Dashboard';

  readonly store = inject(ExpenseService);
  private readonly expenseTableService = inject(ExpenseTableService);

  ngOnInit(): void {
    const tables = this.expenseTableService.tables();
    const target = tables.find(t => t.isStarred) ?? tables[0];
    if (target) {
      this.store.loadAll(target.id);
    }
  }
}
