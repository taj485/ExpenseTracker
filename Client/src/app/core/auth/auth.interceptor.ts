import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '@auth0/auth0-angular';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  if (!req.url.startsWith(environment.apiUrl)) {
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
