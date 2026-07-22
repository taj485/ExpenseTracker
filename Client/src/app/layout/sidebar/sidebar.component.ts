import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
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
}
