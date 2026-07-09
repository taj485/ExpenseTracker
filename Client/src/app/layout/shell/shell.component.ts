import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { AddExpenseFormComponent } from '../../features/add-expense/add-expense-form.component';
import { AddExpenseDrawerService } from '../../core/services/add-expense-drawer.service';

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
  pageTitle    = signal('Dashboard');

  readonly drawer = inject(AddExpenseDrawerService);

  onRouteActivated(component: unknown): void {
    if (component && typeof component === 'object' && 'pageTitle' in component) {
      this.pageTitle.set((component as { pageTitle: string }).pageTitle);
    }
  }
}
