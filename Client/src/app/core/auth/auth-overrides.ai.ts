import { Provider } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';

import { DevAuthService } from './dev-auth.service';

// AI dev mode — replaces auth-overrides.ts only in the "ai" build configuration.
export const authOverrides: Provider[] = [{ provide: AuthService, useClass: DevAuthService }];
