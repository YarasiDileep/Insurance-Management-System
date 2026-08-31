import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor, provideTokenInterceptor } from './token.interceptor';
import { AuthService } from './auth.service';

class MockAuthService {
  token: string | null = null;
  getToken() { return this.token; }
}

describe('TokenInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let auth: MockAuthService;

  beforeEach(() => {
    auth = new MockAuthService();

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: AuthService, useValue: auth },
        // Register the interceptor using the real HTTP_INTERCEPTORS token so
        // the Angular HTTP pipeline executes it during tests.
        { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should NOT add Authorization header when no token', (done) => {
    auth.token = null;

    http.get('/api/Customers').subscribe(() => done());

    const req = httpMock.expectOne('/api/Customers');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush([]);
  });

  it('should add Authorization header when token present', (done) => {
    auth.token = 'abc123';

    http.get('/api/Customers').subscribe(() => done());

    const req = httpMock.expectOne('/api/Customers');
    expect(req.request.headers.get('Authorization')).toBe('Bearer abc123');
    req.flush([]);
  });
});
