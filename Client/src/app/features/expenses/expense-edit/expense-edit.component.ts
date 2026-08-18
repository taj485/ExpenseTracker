import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../core/services/expense.service';
import { ALL_CATEGORIES } from '../../../core/utils/category.utils';
import { ExpenseCategory } from '../../../core/models/expense.model';

@Component({
  selector: 'app-expense-edit',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './expense-edit.component.html',
  styleUrl: './expense-edit.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseEditComponent implements OnInit {
  readonly pageTitle = 'Edit Expense';

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly expenseService = inject(ExpenseService);

  private tableId!: number;
  private id!: number;

  readonly categories = ALL_CATEGORIES;

  readonly loading = signal(true);
  readonly notFound = signal(false);

  unitPrice = signal('');
  quantity = signal('1');
  category = signal<ExpenseCategory>('Food');
  description = signal('');
  merchant = signal('');
  submitting = signal(false);
  formError = signal<string | null>(null);

  ngOnInit(): void {
    this.tableId = Number(this.route.snapshot.paramMap.get('tableId'));
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.expenseService.loadById(
      this.tableId,
      this.id,
      (e) => {
        this.unitPrice.set(String(e.unitPrice));
        this.quantity.set(String(e.quantity));
        this.category.set(e.category);
        this.description.set(e.description);
        this.merchant.set(e.merchant ?? '');
        this.loading.set(false);
      },
      () => {
        this.notFound.set(true);
        this.loading.set(false);
      }
    );
  }

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

    this.formError.set(null);
    this.submitting.set(true);

    this.expenseService.updateExpense(
      this.tableId,
      this.id,
      { unitPrice, quantity, category: this.category(), description, merchant: this.merchant().trim() || null },
      () => {
        this.submitting.set(false);
        this.router.navigate(['/expenses/table', this.tableId, this.id]);
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }

  cancel(): void {
    this.router.navigate(['/expenses/table', this.tableId, this.id]);
  }
}
