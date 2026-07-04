import { TestBed } from '@angular/core/testing';
import { AuthService } from '@auth0/auth0-angular';
import { BehaviorSubject } from 'rxjs';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  let isAuthenticated$: BehaviorSubject<boolean>;
  let isLoading$: BehaviorSubject<boolean>;
  let loginWithRedirect: ReturnType<typeof vi.fn>;

  function setup() {
    isAuthenticated$ = new BehaviorSubject<boolean>(false);
    isLoading$ = new BehaviorSubject<boolean>(false);
    loginWithRedirect = vi.fn();

    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthService,
          useValue: {
            isAuthenticated$,
            isLoading$,
            loginWithRedirect,
          },
        },
      ],
    });
  }

  it('allows activation when the user is authenticated', () => {
    setup();
    isAuthenticated$.next(true);

    let result: boolean | undefined;
    TestBed.runInInjectionContext(() => {
      (authGuard({} as never, {} as never) as any).subscribe((value: boolean) => {
        result = value;
      });
    });

    expect(result).toBe(true);
    expect(loginWithRedirect).not.toHaveBeenCalled();
  });

  it('blocks activation and redirects to login when the user is not authenticated', () => {
    setup();
    isAuthenticated$.next(false);

    let result: boolean | undefined;
    TestBed.runInInjectionContext(() => {
      (authGuard({} as never, {} as never) as any).subscribe((value: boolean) => {
        result = value;
      });
    });

    expect(result).toBe(false);
    expect(loginWithRedirect).toHaveBeenCalledOnce();
  });

  it('waits for the SDK to finish loading before deciding', () => {
    setup();
    isLoading$.next(true);
    isAuthenticated$.next(false);

    let emitted = false;
    TestBed.runInInjectionContext(() => {
      (authGuard({} as never, {} as never) as any).subscribe(() => {
        emitted = true;
      });
    });

    expect(emitted).toBe(false);
    expect(loginWithRedirect).not.toHaveBeenCalled();

    isAuthenticated$.next(true);
    isLoading$.next(false);

    expect(emitted).toBe(true);
  });
});
