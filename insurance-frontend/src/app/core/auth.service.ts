import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, map } from 'rxjs';

interface LoginResponse {
  token: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = '/api/Auth';
  private token$ = new BehaviorSubject<string | null>(localStorage.getItem('jwt'));

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    // Backend expects a POST to /api/Auth/token with a body matching LoginRequest
    // (property names 'Username' and 'Password'). Align the frontend to that shape.
    return this.http.post<LoginResponse>(`${this.apiUrl}/token`, { Username: username, Password: password }).pipe(
      map(res => {
        localStorage.setItem('jwt', res.token);
        this.token$.next(res.token);
        return res.token;
      })
    );
  }

  logout() {
    localStorage.removeItem('jwt');
    this.token$.next(null);
  }

  getToken() {
    return this.token$.value;
  }
}
