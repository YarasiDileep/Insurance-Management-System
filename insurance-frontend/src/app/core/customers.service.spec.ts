import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CustomersService, CustomerDto } from './customers.service';

describe('CustomersService', () => {
  let service: CustomersService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CustomersService]
    });

    service = TestBed.inject(CustomersService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch list of customers', (done) => {
    const mock: CustomerDto[] = [
      { id: 1, name: 'Alice', email: 'alice@example.com' },
      { id: 2, name: 'Bob' }
    ];

    service.list().subscribe(items => {
      expect(items.length).toBe(2);
      expect(items[0].name).toBe('Alice');
      done();
    });

    const req = httpMock.expectOne('/api/Customers');
    expect(req.request.method).toBe('GET');
    req.flush(mock);
  });
});
