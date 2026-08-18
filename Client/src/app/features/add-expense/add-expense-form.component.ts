import { ChangeDetectionStrategy, Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { ALL_CATEGORIES } from '../../core/utils/category.utils';
import { todayLocalISODate } from '../../core/utils/date.utils';
import { AddExpenseCommand, ExpenseCategory } from '../../core/models/expense.model';
import { SelectTablesPromptComponent } from '../expense-table/select-tables-prompt.component';

@Component({
  selector: 'app-add-expense-form',
  standalone: true,
  imports: [FormsModule, SelectTablesPromptComponent],
  templateUrl: './add-expense-form.component.html',
  styleUrl: './add-expense-form.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddExpenseFormComponent {
  @Output() submitted = new EventEmitter<void>();

  private readonly expenseService = inject(ExpenseService);

  readonly categories = ALL_CATEGORIES;

  readonly maxDate = todayLocalISODate();

  readonly step = signal<'form' | 'select-tables'>('form');
  private pendingCommand: Omit<AddExpenseCommand, 'expenseTableId'> | null = null;

  unitPrice   = signal('');
  quantity    = signal('1');
  category    = signal<ExpenseCategory>('Food');
  description = signal('');
  date        = signal(todayLocalISODate());
  merchant    = signal('');
  submitting  = signal(false);
  formError   = signal<string | null>(null);

  onSubmit(): void {
    const unitPrice = parseFloat(this.unitPrice());
    const quantity = parseInt(this.quantity(), 10);
    const description = this.description().trim();

    if (isNaN(unitPrice) || unitPrice <= 0) {
      this.formError.set('Unit price must be greater than zero.');
      return;
    }
    if (isNaN(quantity) || quantity < 1) {
      this.formError.set('Quantity must be at least 1.');
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
    this.pendingCommand = {
      unitPrice,
      quantity,
      category: this.category(),
      description,
      date: this.date(),
      merchant: this.merchant().trim() || null,
    };
    this.step.set('select-tables');
  }

  onTablesConfirmed(tableIds: number[]): void {
    if (!this.pendingCommand) return;

    this.submitting.set(true);
    this.expenseService.addExpenseToTables(
      tableIds,
      this.pendingCommand,
      () => {
        this.submitting.set(false);
        this.pendingCommand = null;
        this.unitPrice.set('');
        this.quantity.set('1');
        this.description.set('');
        this.category.set('Food');
        this.date.set(todayLocalISODate());
        this.merchant.set('');
        this.step.set('form');
        this.submitted.emit();
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }

  onTablesCancelled(): void {
    this.step.set('form');
  }
}
