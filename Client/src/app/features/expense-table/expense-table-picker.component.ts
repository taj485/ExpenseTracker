import { ChangeDetectionStrategy, Component, EventEmitter, Output, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ExpenseTableService } from '../../core/services/expense-table.service';

@Component({
  selector: 'app-expense-table-picker',
  standalone: true,
  templateUrl: './expense-table-picker.component.html',
  styleUrl: './expense-table-picker.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ExpenseTablePickerComponent {
  @Output() selected = new EventEmitter<void>();

  private readonly expenseTableService = inject(ExpenseTableService);
  private readonly router = inject(Router);

  readonly tables = this.expenseTableService.tables;

  selectTable(id: number): void {
    this.router.navigate(['/expenses/table', id]);
    this.selected.emit();
  }
}
