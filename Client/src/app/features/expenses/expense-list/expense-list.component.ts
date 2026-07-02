import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ExpenseService } from '../../../core/services/expense.service';
import { getCategoryMeta } from '../../../core/utils/category.utils';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [DecimalPipe, DatePipe, ConfirmDialogComponent],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseListComponent implements OnInit {
  readonly pageTitle = 'All Expenses';

  readonly store  = inject(ExpenseService);
  readonly router = inject(Router);

  getCategoryMeta = getCategoryMeta;

  readonly deletingId  = signal<number | null>(null);
  readonly actionError = signal<string | null>(null);

  ngOnInit(): void {
    this.store.loadAll();
  }

  viewExpense(id: number): void {
    this.router.navigate(['/expenses', id]);
  }

  editExpense(id: number): void {
    this.router.navigate(['/expenses', id, 'edit']);
  }

  confirmDelete(id: number): void {
    this.actionError.set(null);
    this.store.deleteExpense(
      id,
      () => this.deletingId.set(null),
      (msg) => {
        this.deletingId.set(null);
        this.actionError.set(msg);
      }
    );
  }
}
