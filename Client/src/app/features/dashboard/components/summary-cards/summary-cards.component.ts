import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ExpenseCategory } from '../../../../core/models/expense.model';

@Component({
  selector: 'app-summary-cards',
  standalone: true,
  imports: [DecimalPipe, DatePipe],
  templateUrl: './summary-cards.component.html',
  styleUrl: './summary-cards.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SummaryCardsComponent {
  @Input() thisMonthSpent   = 0;
  @Input() transactionCount = 0;
  @Input() topCategory: { name: ExpenseCategory; total: number } | null = null;

  readonly today = new Date();
}
