import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from '@auth0/auth0-angular';
import { of } from 'rxjs';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;
  let loginWithRedirect: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    loginWithRedirect = vi.fn();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        {
          provide: AuthService,
          useValue: {
            getAccessTokenSilently: () => of('test-token'),
            loginWithRedirect,
          },
        },
      ],
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('attaches the bearer token to /api requests', () => {
    httpClient.get('/api/expense').subscribe();

    const req = httpMock.expectOne('/api/expense');
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush([]);
  });

  it('does not attach a token to non-/api requests', () => {
    httpClient.get('/assets/config.json').subscribe();

    const req = httpMock.expectOne('/assets/config.json');
    expect(req.request.headers.has('Authorization')).toBe(false);
    req.flush({});
  });

  it('redirects to login and propagates the error on a 401 from the API', () => {
    let errored = false;

    httpClient.get('/api/expense').subscribe({
      next: () => {},
      error: () => { errored = true; },
    });

    const req = httpMock.expectOne('/api/expense');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(loginWithRedirect).toHaveBeenCalledOnce();
    expect(errored).toBe(true);
  });
});
