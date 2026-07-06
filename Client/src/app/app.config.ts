import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AuthService, provideAuth0 } from '@auth0/auth0-angular';
import { catchError, firstValueFrom, of } from 'rxjs';

import { routes } from './app.routes';
import { environment } from '../environments/environment';
import { authInterceptor } from './core/auth/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAuth0({
      domain: environment.auth0.domain,
      clientId: environment.auth0.clientId,
      useRefreshTokens: true,
      useRefreshTokensFallback: true,
      cacheLocation: 'memory',
      authorizationParams: {
        redirect_uri: window.location.origin,
        audience: environment.auth0.audience,
      },
    }),
    // Restores the session on a fresh page load by silently checking Auth0
    // (via SSO cookie) before routing/guards run, since the in-memory token
    // cache is always empty at that point.
    provideAppInitializer(() => {
      const auth = inject(AuthService);
      return firstValueFrom(auth.getAccessTokenSilently().pipe(catchError(() => of(null))));
    }),
  ],
};
