import { Routes } from '@angular/router';
import { AuthGuard } from './core/auth.guard';
import { LoginGuard } from './core/login.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [LoginGuard],
    loadComponent: () =>
      import('./feature/auth/components/login/login.component').then(
        (m) => m.LoginComponent
      ),
  },
  {
    path: 'register',
    canActivate: [LoginGuard],
    loadComponent: () =>
      import('./feature/auth/components/register/register.component').then(
        (m) => m.RegisterComponent
      ),
  },
  {
    path: 'email-confirmed',
    canActivate: [LoginGuard],
    loadComponent: () =>
      import(
        './feature/auth/components/email-confirmed/email-confirmed.component'
      ).then((m) => m.EmailConfirmedComponent),
  },
  {
    path: 'home',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./feature/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: '/login',
  },
];
