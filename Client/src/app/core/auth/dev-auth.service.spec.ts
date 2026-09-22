import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { authOverrides } from './auth-overrides';
import { DevAuthService } from './dev-auth.service';

describe('authOverrides', () => {
  it('replaces nothing outside the ai build configuration', () => {
    expect(authOverrides).toEqual([]);
  });
});

describe('DevAuthService', () => {
  let service: DevAuthService;
  let navigate: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    navigate = vi.fn().mockResolvedValue(true);
    TestBed.configureTestingModule({
      providers: [DevAuthService, { provide: Router, useValue: { navigate } }],
    });
    service = TestBed.inject(DevAuthService);
  });

  it('starts signed in', async () => {
    expect(await firstValueFrom(service.isAuthenticated$)).toBe(true);
  });

  it('signs out and returns to the landing page on logout', async () => {
    service.logout();

    expect(await firstValueFrom(service.isAuthenticated$)).toBe(false);
    expect(navigate).toHaveBeenCalledWith(['/']);
  });

  it('signs back in and opens the dashboard on login', async () => {
    service.logout();
    service.loginWithRedirect();

    expect(await firstValueFrom(service.isAuthenticated$)).toBe(true);
    expect(navigate).toHaveBeenLastCalledWith(['/dashboard']);
  });
});
