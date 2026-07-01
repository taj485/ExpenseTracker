import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { CategoryStat } from '../../../../core/models/expense.model';
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

  getCategoryMeta = getCategoryMeta;
}
