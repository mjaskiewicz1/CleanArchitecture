import { Routes } from '@angular/router';
import { Login } from './pages/login/login';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: Login,
  },
  {
    path: '',
    loadComponent: () => import('./pages/layout/layout').then(m => m.Layout),
    children: [
      {
        path: 'users',
        loadComponent: () => import('./pages/users/user-list/user-list').then(m => m.UserList),
      },
      {
        path: 'users/create',
        loadComponent: () => import('./pages/users/user-create/user-create').then(m => m.UserCreate),
      },
      {
        path: 'users/:id/edit',
        loadComponent: () => import('./pages/users/user-edit/user-edit').then(m => m.UserEdit),
      },
      {
        path: 'users/:id/delete',
        loadComponent: () => import('./pages/users/user-delete/user-delete').then(m => m.UserDelete),
      },
      {
        path: 'settings',
        loadComponent: () => import('./pages/settings/settings').then(m => m.Settings),
      },
    ],
  },
];
