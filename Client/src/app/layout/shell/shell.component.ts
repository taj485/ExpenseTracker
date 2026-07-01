import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { AddExpenseFormComponent } from '../../features/add-expense/add-expense-form.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent, AddExpenseFormComponent],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShellComponent {
  sidebarOpen  = signal(false);
  drawerOpen   = signal(false);
  pageTitle    = signal('Dashboard');

  onRouteActivated(component: unknown): void {
    if (component && typeof component === 'object' && 'pageTitle' in component) {
      this.pageTitle.set((component as { pageTitle: string }).pageTitle);
    }
  }
}
