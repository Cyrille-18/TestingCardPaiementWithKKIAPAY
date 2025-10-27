import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';

/**
 * Service for authentication-related API calls
 * Handles login, registration, email confirmation, etc.
 */
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = `${environment.apiBaseUrl}/auth`;
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'current_user';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(
    this.hasValidToken()
  );
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  /**
   * Check if user has a valid token
   * Using sessionStorage instead of localStorage for better security
   */
  private hasValidToken(): boolean {
    const token = sessionStorage.getItem(this.TOKEN_KEY);
    return !!token;
  }

  /**
   * Get current user from sessionStorage
   */
  getCurrentUser(): any {
    const userStr = sessionStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  /**
   * Get auth token from sessionStorage
   */
  getToken(): string | null {
    return sessionStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Get refresh token from sessionStorage
   */
  getRefreshToken(): string | null {
    return sessionStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Confirms user email with userId and token
   * @param userId The user ID
   * @param token The confirmation token
   * @returns Observable of the HTTP response
   */
  confirmEmail(userId: string, token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/confirm-email`, {
      params: { userId, token },
      observe: 'response',
    });
  }

  /**
   * Logs in a user
   * @param email User email
   * @param password User password
   * @returns Observable of the authentication response
   */
  login(email: string, password: string): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/login`, {
        email,
        password,
      })
      .pipe(
        tap((response: any) => {
          if (response.token && response.user) {
            // Store token and user data in sessionStorage for better security
            sessionStorage.setItem(this.TOKEN_KEY, response.token);
            sessionStorage.setItem(
              this.USER_KEY,
              JSON.stringify(response.user)
            );

            // Store refresh token if provided
            if (response.refreshToken) {
              sessionStorage.setItem(
                this.REFRESH_TOKEN_KEY,
                response.refreshToken
              );
            }

            // Update authentication state
            this.isAuthenticatedSubject.next(true);

            // Redirect to home page
            this.router.navigate(['/home']);
          }
        })
      );
  }

  /**
   * Logs out the current user
   */
  logout(): void {
    // Clear stored data from sessionStorage
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);
    sessionStorage.removeItem(this.REFRESH_TOKEN_KEY);

    // Update authentication state
    this.isAuthenticatedSubject.next(false);

    // Redirect to login page
    this.router.navigate(['/login']);
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  /**
   * Refresh authentication token
   */
  refreshToken(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    return this.http.post(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap((response: any) => {
        if (response.token) {
          sessionStorage.setItem(this.TOKEN_KEY, response.token);
          // Update refresh token if provided
          if (response.refreshToken) {
            sessionStorage.setItem(
              this.REFRESH_TOKEN_KEY,
              response.refreshToken
            );
          }
        }
      })
    );
  }

  /**
   * Registers a new user
   * @param userData User registration data
   * @returns Observable of the registration response
   */
  register(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }
}
