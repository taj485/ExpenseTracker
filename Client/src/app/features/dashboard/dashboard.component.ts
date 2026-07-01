import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { ExpenseService } from '../../core/services/expense.service';
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

  ngOnInit(): void {
    this.store.loadAll();
  }
}
