import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { ExpenseTableService } from '../../core/services/expense-table.service';

@Component({
  selector: 'app-select-tables-prompt',
  standalone: true,
  templateUrl: './select-tables-prompt.component.html',
  styleUrl: './select-tables-prompt.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SelectTablesPromptComponent {
  private readonly expenseTableService = inject(ExpenseTableService);
  readonly tables = this.expenseTableService.tables;

  readonly selectedIds = signal<Set<number>>(new Set());

  @Input() submitting = false;
  @Output() confirmed = new EventEmitter<number[]>();
  @Output() cancelled = new EventEmitter<void>();

  toggle(id: number): void {
    this.selectedIds.update(set => {
      const next = new Set(set);
      if (next.has(id)) next.delete(id); else next.add(id);
      return next;
    });
  }

  confirm(): void {
    if (this.selectedIds().size === 0) return;
    this.confirmed.emit([...this.selectedIds()]);
  }
}
