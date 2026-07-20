import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { Expense } from '../../../core/models/expense.model';
import { ExpenseService } from '../../../core/services/expense.service';
import { getCategoryMeta } from '../../../core/utils/category.utils';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-receipt-group',
  standalone: true,
  imports: [DecimalPipe, DatePipe, ConfirmDialogComponent],
  templateUrl: './receipt-group.component.html',
  styleUrl: './receipt-group.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReceiptGroupComponent implements OnInit {
  readonly pageTitle = 'Receipt Group';

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly expenseService = inject(ExpenseService);

  readonly items = signal<Expense[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly confirmingGroupDelete = signal(false);
  readonly actionError = signal<string | null>(null);

  getCategoryMeta = getCategoryMeta;

  readonly totalAmount = computed(() => this.items().reduce((sum, i) => sum + i.amount, 0));

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.loadGroup(Number(params.get('receiptId')));
    });
  }

  private loadGroup(receiptId: number): void {
    this.loading.set(true);
    this.error.set(null);
    this.items.set([]);
    this.confirmingGroupDelete.set(false);
    this.actionError.set(null);

    this.expenseService.loadByReceiptId(
      receiptId,
      (items) => {
        if (items.length === 0) {
          this.error.set('Receipt not found.');
        } else {
          this.items.set(items);
        }
        this.loading.set(false);
      },
      () => { this.error.set('Receipt not found.'); this.loading.set(false); }
    );
  }

  goBack(): void {
    this.router.navigate(['/expenses']);
  }

  viewItem(id: number): void {
    this.router.navigate(['/expenses', id]);
  }

  editItem(id: number): void {
    this.router.navigate(['/expenses', id, 'edit']);
  }

  confirmGroupDelete(): void {
    const ids = this.items().map(i => i.id);
    if (ids.length === 0) return;

    this.actionError.set(null);
    this.expenseService.deleteExpenses(
      ids,
      () => this.router.navigate(['/expenses']),
      (msg) => {
        this.confirmingGroupDelete.set(false);
        this.actionError.set(msg);
      }
    );
  }
}
