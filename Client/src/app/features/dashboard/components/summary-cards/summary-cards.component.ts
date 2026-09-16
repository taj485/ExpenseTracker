import { ChangeDetectionStrategy, Component, Input, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { ExpenseCategory } from '../../../../core/models/expense.model';

@Component({
  selector: 'app-summary-cards',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './summary-cards.component.html',
  styleUrl: './summary-cards.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SummaryCardsComponent {
  @Input() monthSpent       = 0;
  @Input() transactionCount = 0;
  @Input() topCategory: { name: ExpenseCategory; total: number } | null = null;
  /** Already-formatted month, e.g. 'August 2026'. */
  @Input() monthLabel = '';
  /** Table and 'YYYY-MM' month the cards describe — where a card click takes the user. */
  @Input() tableId: number | null = null;
  @Input() month = '';

  private readonly router = inject(Router);

  /** Opens the expense list filtered to this month, and to a category when given. */
  viewExpenses(category?: ExpenseCategory): void {
    if (this.tableId === null) return;

    this.router.navigate(['/expenses/table', this.tableId], {
      queryParams: { month: this.month || null, category: category ?? null },
    });
  }
}
