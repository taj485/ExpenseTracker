import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { AddExpenseFormComponent } from '../../features/add-expense/add-expense-form.component';
import { AddExpenseDrawerService } from '../../core/services/add-expense-drawer.service';
import { UploadReceiptComponent } from '../../features/upload-receipt/upload-receipt.component';
import { UploadReceiptDrawerService } from '../../core/services/upload-receipt-drawer.service';
import { DragToDismissDirective } from '../../shared/drag-to-dismiss.directive';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { CreateExpenseTablePromptComponent } from '../../features/expense-table/create-expense-table-prompt.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SidebarComponent, TopbarComponent, AddExpenseFormComponent, UploadReceiptComponent, DragToDismissDirective, CreateExpenseTablePromptComponent],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShellComponent implements OnInit {
  sidebarOpen  = signal(false);
  pageTitle    = signal('Dashboard');

  readonly drawer = inject(AddExpenseDrawerService);
  readonly uploadDrawer = inject(UploadReceiptDrawerService);
  readonly expenseTableService = inject(ExpenseTableService);

  ngOnInit(): void {
    this.expenseTableService.getTables();
  }

  onRouteActivated(component: unknown): void {
    if (component && typeof component === 'object' && 'pageTitle' in component) {
      this.pageTitle.set((component as { pageTitle: string }).pageTitle);
    }
  }
}
