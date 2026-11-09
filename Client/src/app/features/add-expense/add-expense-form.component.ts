import { ChangeDetectionStrategy, Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { ALL_CATEGORIES } from '../../core/utils/category.utils';
import { todayLocalISODate } from '../../core/utils/date.utils';
import { AddExpenseCommand, ExpenseCategory, QuantityUnit } from '../../core/models/expense.model';
import { QUANTITY_UNITS, quantityError, quantityStep } from '../../core/utils/quantity.utils';
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
  readonly units = QUANTITY_UNITS;
  readonly quantityStep = quantityStep;

  readonly maxDate = todayLocalISODate();

  readonly step = signal<'form' | 'select-tables'>('form');
  private pendingCommand: Omit<AddExpenseCommand, 'expenseTableId'> | null = null;

  unitPrice   = signal('');
  quantity    = signal('1');
  unit        = signal<QuantityUnit>('Item');
  category    = signal<ExpenseCategory>('Food');
  description = signal('');
  date        = signal(todayLocalISODate());
  merchant    = signal('');
  submitting  = signal(false);
  formError   = signal<string | null>(null);

  onSubmit(): void {
    const unitPrice = parseFloat(this.unitPrice());
    const quantity = parseFloat(this.quantity());
    const description = this.description().trim();

    if (isNaN(unitPrice) || unitPrice <= 0) {
      this.formError.set('Unit price must be greater than zero.');
      return;
    }
    const quantityProblem = quantityError(quantity, this.unit());
    if (quantityProblem) {
      this.formError.set(quantityProblem);
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
      unit: this.unit(),
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
        this.unit.set('Item');
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
