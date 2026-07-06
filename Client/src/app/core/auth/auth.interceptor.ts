import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '@auth0/auth0-angular';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  if (!req.url.startsWith('/api')) {
    return next(req);
  }

  return auth.getAccessTokenSilently().pipe(
    switchMap((token) =>
      next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }))
    ),
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401) {
        auth.loginWithRedirect();
      }
      return throwError(() => err);
    })
  );
};
