import { Provider } from '@angular/core';

/**
 * Providers that replace real authentication. Empty in every build except
 * `ng serve --configuration ai`, which swaps in auth-overrides.ai.ts (see angular.json).
 */
export const authOverrides: Provider[] = [];
