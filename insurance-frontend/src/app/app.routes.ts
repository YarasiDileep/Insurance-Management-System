import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent),
  },
  { 
    path: 'policies', 
    loadComponent: () => import('./pages/policies/policies.component').then(m => m.PoliciesComponent) 
  },
  { 
    path: 'claims', 
    loadComponent: () => import('./pages/claims/claims.component').then(m => m.ClaimsComponent) 
  },
];
