import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { combineLatest } from 'rxjs';
import { filter, take } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly checking = signal(true);

  ngOnInit(): void {
    combineLatest([this.auth.isAuthenticated$, this.auth.isLoading$])
      .pipe(
        filter(([, isLoading]) => !isLoading),
        take(1)
      )
      .subscribe(([isAuthenticated]) => {
        if (isAuthenticated) {
          this.router.navigate(['/dashboard']);
        } else {
          this.checking.set(false);
        }
      });
  }

  login(): void {
    this.auth.loginWithRedirect();
  }
}
