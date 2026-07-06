import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell.component';
import { HomeComponent } from './features/home/home.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ExpenseListComponent } from './features/expenses/expense-list/expense-list.component';
import { ExpenseDetailComponent } from './features/expenses/expense-detail/expense-detail.component';
import { ExpenseEditComponent } from './features/expenses/expense-edit/expense-edit.component';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard',         component: DashboardComponent },
      { path: 'expenses',          component: ExpenseListComponent },
      { path: 'expenses/:id/edit', component: ExpenseEditComponent },
      { path: 'expenses/:id',      component: ExpenseDetailComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
