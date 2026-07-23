import { ChangeDetectionStrategy, Component, Input, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { CategoryStat, ExpenseCategory } from '../../../../core/models/expense.model';
import { getCategoryMeta } from '../../../../core/utils/category.utils';

@Component({
  selector: 'app-category-breakdown',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './category-breakdown.component.html',
  styleUrl: './category-breakdown.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoryBreakdownComponent {
  @Input() stats: CategoryStat[] = [];
  @Input() tableId: number | null = null;

  private readonly router = inject(Router);

  getCategoryMeta = getCategoryMeta;

  viewCategory(category: ExpenseCategory): void {
    if (this.tableId === null) return;

    const now = new Date();
    const month = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
    this.router.navigate(['/expenses/table', this.tableId], { queryParams: { category, month } });
  }
}
