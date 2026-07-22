import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ExpenseTableService } from '../../core/services/expense-table.service';

@Component({
  selector: 'app-create-expense-table-prompt',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './create-expense-table-prompt.component.html',
  styleUrl: './create-expense-table-prompt.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateExpenseTablePromptComponent {
  private readonly expenseTableService = inject(ExpenseTableService);

  name = signal('');
  submitting = signal(false);
  formError = signal<string | null>(null);

  onSubmit(): void {
    const name = this.name().trim();

    if (!name) {
      this.formError.set('Name is required.');
      return;
    }
    if (name.length > 200) {
      this.formError.set('Name is too long.');
      return;
    }

    this.formError.set(null);
    this.submitting.set(true);

    this.expenseTableService.createTable(
      { name },
      () => {
        this.submitting.set(false);
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }
}
