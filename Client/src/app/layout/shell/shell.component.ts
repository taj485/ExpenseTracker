import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { AddExpenseFormComponent } from '../../features/add-expense/add-expense-form.component';
import { AddExpenseDrawerService } from '../../core/services/add-expense-drawer.service';
import { UploadReceiptComponent } from '../../features/upload-receipt/upload-receipt.component';
import { UploadReceiptDrawerService } from '../../core/services/upload-receipt-drawer.service';
import { DragToDismissDirective } from '../../shared/drag-to-dismiss.directive';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { CreateExpenseTablePromptComponent } from '../../features/expense-table/create-expense-table-prompt.component';
import { CreateExpenseTableDialogService } from '../../core/services/create-expense-table-dialog.service';
import { ExpenseTablePickerComponent } from '../../features/expense-table/expense-table-picker.component';
import { ExpenseTablePickerDrawerService } from '../../core/services/expense-table-picker-drawer.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SidebarComponent, TopbarComponent, AddExpenseFormComponent, UploadReceiptComponent, DragToDismissDirective, CreateExpenseTablePromptComponent, ExpenseTablePickerComponent],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShellComponent implements OnInit {
  sidebarOpen  = signal(false);
  activatedComponent = signal<unknown>(null);

  readonly pageTitle = computed(() => {
    const component = this.activatedComponent();
    if (component && typeof component === 'object' && 'pageTitle' in component) {
      const title = (component as { pageTitle: unknown }).pageTitle;
      return typeof title === 'function' ? title() : String(title);
    }
    return 'Dashboard';
  });

  readonly drawer = inject(AddExpenseDrawerService);
  readonly uploadDrawer = inject(UploadReceiptDrawerService);
  readonly expenseTableService = inject(ExpenseTableService);
  readonly createTableDialogService = inject(CreateExpenseTableDialogService);
  readonly tablePickerDrawer = inject(ExpenseTablePickerDrawerService);
  readonly router = inject(Router);

  ngOnInit(): void {
    this.expenseTableService.getTables();
  }

  onRouteActivated(component: unknown): void {
    this.activatedComponent.set(component);
  }

  onExpensesTabClick(): void {
    const tables = this.expenseTableService.tables();
    if (tables.length === 1) {
      this.router.navigate(['/expenses/table', tables[0].id]);
    } else {
      this.tablePickerDrawer.open();
    }
  }
}
