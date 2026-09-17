import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ExpenseTable } from '../../core/models/expense-table.model';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { CreateExpenseTableDialogService } from '../../core/services/create-expense-table-dialog.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();

  readonly expenseTableService = inject(ExpenseTableService);
  readonly createTableDialogService = inject(CreateExpenseTableDialogService);
  private readonly expenseService = inject(ExpenseService);

  /**
   * Expense count for a space, when it's the one whose expenses are loaded — the expense store
   * holds a single space at a time, so other spaces show no count rather than a wrong one.
   */
  spaceCount(tableId: number): number | null {
    return this.expenseService.loadedTableId() === tableId ? this.expenseService.expenses().length : null;
  }

  /** The web app won't unstar the only space (the button is disabled then). */
  toggleStar(table: ExpenseTable): void {
    const ignore = () => {};
    if (table.isStarred) {
      this.expenseTableService.unstarTable(table.id, ignore, ignore);
    } else {
      this.expenseTableService.starTable(table.id, ignore, ignore);
    }
  }
}
