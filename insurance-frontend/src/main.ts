import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { App } from './app/app';
import { provideTokenInterceptor } from './app/core/token.interceptor';

bootstrapApplication(App, {
  providers: [
    provideHttpClient(),
    provideRouter(routes),
    importProvidersFrom(),
    provideTokenInterceptor,
  ],
}).catch(err => console.error(err));
