import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ExpenseService } from '../../../core/services/expense.service';
import { getCategoryMeta } from '../../../core/utils/category.utils';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [DecimalPipe, DatePipe],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseListComponent implements OnInit {
  readonly pageTitle = 'All Expenses';

  readonly store  = inject(ExpenseService);
  readonly router = inject(Router);

  getCategoryMeta = getCategoryMeta;

  ngOnInit(): void {
    this.store.loadAll();
  }

  viewExpense(id: number): void {
    this.router.navigate(['/expenses', id]);
  }
}
