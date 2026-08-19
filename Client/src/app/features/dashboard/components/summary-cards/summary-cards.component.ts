import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { DecimalPipe } from '@angular/common';
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
}
