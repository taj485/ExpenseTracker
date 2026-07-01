import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { Expense } from '../../../core/models/expense.model';
import { ExpenseService } from '../../../core/services/expense.service';
import { getCategoryMeta } from '../../../core/utils/category.utils';

@Component({
  selector: 'app-expense-detail',
  standalone: true,
  imports: [DecimalPipe, DatePipe],
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseDetailComponent implements OnInit {
  readonly pageTitle = 'Expense Detail';

  private readonly route  = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly expenseService = inject(ExpenseService);

  readonly expense = signal<Expense | null>(null);
  readonly loading = signal(true);
  readonly error   = signal<string | null>(null);

  getCategoryMeta = getCategoryMeta;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.expenseService.loadById(
      id,
      (e) => { this.expense.set(e); this.loading.set(false); },
      ()  => { this.error.set('Expense not found.'); this.loading.set(false); }
    );
  }

  goBack(): void {
    this.router.navigate(['/expenses']);
  }
}
