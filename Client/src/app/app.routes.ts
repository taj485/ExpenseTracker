import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ExpenseListComponent } from './features/expenses/expense-list/expense-list.component';
import { ExpenseDetailComponent } from './features/expenses/expense-detail/expense-detail.component';

export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '',             component: DashboardComponent },
      { path: 'expenses',     component: ExpenseListComponent },
      { path: 'expenses/:id', component: ExpenseDetailComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
