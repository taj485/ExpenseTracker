import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { Expense } from '../../../core/models/expense.model';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { getCategoryMeta } from '../../../core/utils/category.utils';
import { triggerBlobDownload } from '../../../core/utils/download.utils';
import { uploaderLabel } from '../../../core/utils/uploader.utils';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-expense-detail',
  standalone: true,
  imports: [DecimalPipe, DatePipe, ConfirmDialogComponent],
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseDetailComponent implements OnInit {
  readonly pageTitle = 'Expense Detail';

  private readonly route  = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly expenseService = inject(ExpenseService);
  private readonly expenseTableService = inject(ExpenseTableService);

  readonly expense = signal<Expense | null>(null);
  private readonly tableIdSignal = signal<number | null>(null);

  /** "You" or the uploader's email — only in shared spaces, where it isn't always you. */
  readonly addedBy = computed(() => {
    const e = this.expense();
    const table = this.expenseTableService.tables().find(t => t.id === this.tableIdSignal());
    if (!e || !table || table.memberCount <= 1) return null;
    return uploaderLabel(e, 'full');
  });
  readonly loading = signal(true);
  readonly error   = signal<string | null>(null);

  readonly confirmingDelete = signal(false);
  readonly actionError = signal<string | null>(null);

  private tableId!: number;

  getCategoryMeta = getCategoryMeta;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.tableId = Number(params.get('tableId'));
      this.tableIdSignal.set(this.tableId);
      this.loadExpense(Number(params.get('id')));
    });
  }

  private loadExpense(id: number): void {
    this.loading.set(true);
    this.error.set(null);
    this.expense.set(null);
    this.confirmingDelete.set(false);
    this.actionError.set(null);

    this.expenseService.loadById(
      this.tableId,
      id,
      (e) => {
        this.expense.set(e);
        this.loading.set(false);
      },
      () => { this.error.set('Expense not found.'); this.loading.set(false); }
    );
  }

  goBack(): void {
    this.router.navigate(['/expenses/table', this.tableId]);
  }

  editExpense(): void {
    const e = this.expense();
    if (e) this.router.navigate(['/expenses/table', this.tableId, e.id, 'edit']);
  }

  downloadReceipt(): void {
    const e = this.expense();
    if (!e || e.receiptId === null) return;

    this.actionError.set(null);
    this.expenseService.downloadReceiptImage(
      this.tableId,
      e.receiptId,
      (blob, filename) => triggerBlobDownload(blob, filename),
      (msg) => this.actionError.set(msg),
    );
  }

  confirmDelete(): void {
    const e = this.expense();
    if (!e) return;

    this.actionError.set(null);
    this.expenseService.deleteExpense(
      this.tableId,
      e.id,
      () => this.router.navigate(['/expenses/table', this.tableId]),
      (msg) => {
        this.confirmingDelete.set(false);
        this.actionError.set(msg);
      }
    );
  }
}
