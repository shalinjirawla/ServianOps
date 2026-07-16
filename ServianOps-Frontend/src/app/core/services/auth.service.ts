import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of, map } from 'rxjs';

import { LoginDto, AuthResponseDto, UserSession } from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Core signals to manage application auth state
  readonly currentUser = signal<UserSession | null>(null);
  readonly isAuthenticated = computed(() => this.currentUser() !== null);
  readonly userRole = computed(() => this.currentUser()?.role || null);
  readonly tenantId = computed(() => this.currentUser()?.tenantId || null);

  constructor() {}

  /**
   * Initializes the session from persistent storage on startup by calling /me endpoint
   */
  initSession(): Observable<any> {
    return this.http.get<{ data: AuthResponseDto }>(`${environment.apiUrl}/api/auth/me`, { withCredentials: true })
      .pipe(
        catchError(() => {
          this.clearSessionState();
          return of(null);
        }),
        tap((res) => {
          if (res && res.data) {
            const userSession: UserSession = {
              userId: res.data.userId,
              tenantId: res.data.tenantId,
              email: res.data.email,
              role: res.data.role
            };
            this.currentUser.set(userSession);
          } else {
            this.clearSessionState();
          }
        })
      );
  }

  /**
   * Log in user using the backend Unified Login API
   */
  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<{ data: AuthResponseDto }>(`${environment.apiUrl}/api/auth/login`, dto, { withCredentials: true }).pipe(
      map(res => res.data),
      tap((res) => {
        if (res) {
          const userSession: UserSession = {
            userId: res.userId,
            tenantId: res.tenantId,
            email: res.email,
            role: res.role
          };
          this.currentUser.set(userSession);
        }
      })
    );
  }

  /**
   * Logs out the user client-side and redirects to login page
   */
  logout(reason?: string): void {
    if (this.isAuthenticated()) {
      this.http.post(`${environment.apiUrl}/api/auth/logout`, {}, { withCredentials: true }).subscribe({
        next: () => { },
        error: () => { }
      });
    }

    this.clearSessionState();
    const queryParams = reason ? { reason } : {};
    this.router.navigate(['/auth/login'], { queryParams });
  }

  /**
   * Forces client-side session cleanup
   */
  clearSessionState(): void {
    this.currentUser.set(null);
    try {
      sessionStorage.clear();
    } catch { }
  }

  /**
   * Send forgot password request
   */
  forgotPassword(email: string, tenancyName?: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/auth/forgot-password`, { email, tenancyName });
  }
}
