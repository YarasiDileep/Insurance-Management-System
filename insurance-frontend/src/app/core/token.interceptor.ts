import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { AuthService } from './auth.service';
import { ENVIRONMENT_INITIALIZER } from '@angular/core';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = this.auth.getToken();
    if (!token) return next.handle(req);

    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });

    return next.handle(cloned);
  }
}

export const provideTokenInterceptor = {
  provide: 'HTTP_INTERCEPTORS',
  useClass: TokenInterceptor,
  multi: true,
};
