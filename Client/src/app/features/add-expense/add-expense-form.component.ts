import { ChangeDetectionStrategy, Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { ALL_CATEGORIES } from '../../core/utils/category.utils';
import { ExpenseCategory } from '../../core/models/expense.model';

function todayLocalISODate(): string {
  const d = new Date();
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

@Component({
  selector: 'app-add-expense-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-expense-form.component.html',
  styleUrl: './add-expense-form.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddExpenseFormComponent {
  @Output() submitted = new EventEmitter<void>();

  private readonly expenseService = inject(ExpenseService);

  readonly categories = ALL_CATEGORIES;

  readonly maxDate = todayLocalISODate();

  amount      = signal('');
  category    = signal<ExpenseCategory>('Food');
  description = signal('');
  date        = signal(todayLocalISODate());
  submitting  = signal(false);
  formError   = signal<string | null>(null);

  onSubmit(): void {
    const amount = parseFloat(this.amount());
    const description = this.description().trim();

    if (isNaN(amount) || amount <= 0) {
      this.formError.set('Amount must be greater than zero.');
      return;
    }
    if (!description) {
      this.formError.set('Description is required.');
      return;
    }
    if (this.date() > this.maxDate) {
      this.formError.set('Date cannot be in the future.');
      return;
    }

    this.formError.set(null);
    this.submitting.set(true);

    this.expenseService.addExpense(
      { amount, category: this.category(), description, date: this.date() },
      () => {
        this.submitting.set(false);
        this.amount.set('');
        this.description.set('');
        this.category.set('Food');
        this.date.set(todayLocalISODate());
        this.submitted.emit();
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }
}
