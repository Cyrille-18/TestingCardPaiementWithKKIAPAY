import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../feature/auth/services/auth.service';

/**
 * Guard to redirect authenticated users away from login page
 * Redirects to home page if user is already authenticated
 */
@Injectable({
  providedIn: 'root',
})
export class LoginGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      // Redirect to home page if already authenticated
      this.router.navigate(['/home']);
      return false;
    } else {
      return true;
    }
  }
}
