import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { combineLatest } from 'rxjs';
import { filter, take } from 'rxjs/operators';
import { LandingHeroComponent } from './components/landing-hero/landing-hero.component';
import { LandingFeaturesComponent } from './components/landing-features/landing-features.component';
import { LandingInsightsComponent } from './components/landing-insights/landing-insights.component';
import { LandingStepsComponent } from './components/landing-steps/landing-steps.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [LandingHeroComponent, LandingFeaturesComponent, LandingInsightsComponent, LandingStepsComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./components/landing-shared.css', './home.component.css'],
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
