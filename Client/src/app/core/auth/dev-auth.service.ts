import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';

/**
 * AI dev mode stand-in for Auth0's AuthService, so the app runs without a login.
 * Starts signed in; login and logout just flip that and navigate, like the real flow.
 * The API's DevBypass scheme accepts the placeholder token. Only provided by
 * auth-overrides.ai.ts, so it never reaches a production bundle.
 */
@Injectable()
export class DevAuthService {
  private readonly router = inject(Router);
  private readonly signedIn = new BehaviorSubject(true);

  readonly isAuthenticated$ = this.signedIn.asObservable();
  readonly isLoading$ = of(false);

  getAccessTokenSilently(): Observable<string> {
    return of('ai-dev-mode');
  }

  loginWithRedirect(): Observable<void> {
    this.signedIn.next(true);
    void this.router.navigate(['/dashboard']);
    return of(undefined);
  }

  logout(): Observable<void> {
    this.signedIn.next(false);
    void this.router.navigate(['/']);
    return of(undefined);
  }
}
