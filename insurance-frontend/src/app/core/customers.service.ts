import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CustomerDto {
  id: number;
  name: string;
  email?: string;
}

@Injectable({ providedIn: 'root' })
export class CustomersService {
  private api = '/api/Customers';
  constructor(private http: HttpClient) {}

  list(): Observable<CustomerDto[]> {
    return this.http.get<CustomerDto[]>(this.api);
  }
}
