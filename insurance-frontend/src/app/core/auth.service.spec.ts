import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    // ensure localStorage is clean for tests
    localStorage.removeItem('jwt');
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.removeItem('jwt');
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call login and store token', (done) => {
    const dummyToken = 'abc.def.ghi';

    service.login('user', 'pass').subscribe(token => {
      expect(token).toBe(dummyToken);
      expect(localStorage.getItem('jwt')).toBe(dummyToken);
      expect(service.getToken()).toBe(dummyToken);
      done();
    });

    const req = httpMock.expectOne('/api/Auth/token');
    expect(req.request.method).toBe('POST');
    // backend expects Username/Password keys
    expect(req.request.body).toEqual({ Username: 'user', Password: 'pass' });
    req.flush({ token: dummyToken });
  });

  it('should logout and clear token', () => {
    localStorage.setItem('jwt', 'x.y.z');
    service.logout();
    expect(localStorage.getItem('jwt')).toBeNull();
    expect(service.getToken()).toBeNull();
  });
});
