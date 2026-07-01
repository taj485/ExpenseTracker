import { ChangeDetectionStrategy, Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { ALL_CATEGORIES } from '../../core/utils/category.utils';
import { ExpenseCategory } from '../../core/models/expense.model';

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

  amount      = signal('');
  category    = signal<ExpenseCategory>('Food');
  description = signal('');
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

    this.formError.set(null);
    this.submitting.set(true);

    this.expenseService.addExpense(
      { amount, category: this.category(), description },
      () => {
        this.submitting.set(false);
        this.amount.set('');
        this.description.set('');
        this.category.set('Food');
        this.submitted.emit();
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }
}
