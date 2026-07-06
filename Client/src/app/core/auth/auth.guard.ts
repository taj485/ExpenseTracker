import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { combineLatest } from 'rxjs';
import { filter, map, take, tap } from 'rxjs/operators';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);

  return combineLatest([auth.isAuthenticated$, auth.isLoading$]).pipe(
    filter(([, isLoading]) => !isLoading),
    take(1),
    map(([isAuthenticated]) => isAuthenticated),
    tap((isAuthenticated) => {
      if (!isAuthenticated) {
        auth.loginWithRedirect();
      }
    })
  );
};
