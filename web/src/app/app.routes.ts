import { Routes } from '@angular/router';
import { Login } from './pages/login/login/login';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard/dashboard').then(m => m.Dashboard)
  },
];
