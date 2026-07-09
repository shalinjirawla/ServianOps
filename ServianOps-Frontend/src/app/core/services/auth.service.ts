import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginDto, AuthResponseDto, UserSession } from '../models/auth.models';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);

  // Core signals to manage application auth state
  readonly currentUser = signal<UserSession | null>(null);
  readonly isAuthenticated = computed(() => this.currentUser() !== null);
  readonly userRole = computed(() => this.currentUser()?.role || null);
  readonly tenantId = computed(() => this.currentUser()?.tenantId || null);

  constructor() {
    this.initSession();
  }

  /**
   * Initializes the session from persistent storage on startup
   */
  initSession(): void {
    const token = this.tokenService.getToken();
    if (token) {
      if (this.tokenService.isTokenExpired(token)) {
        this.clearSessionState();
      } else {
        const decoded = this.tokenService.decodeToken(token);
        if (decoded) {
          const userSession: UserSession = {
            token,
            userId: Number(decoded.user_id),
            tenantId: decoded.tenant_id ? Number(decoded.tenant_id) : null,
            email: decoded.email,
            role: decoded.role,
            decodedToken: decoded
          };
          this.currentUser.set(userSession);
        } else {
          this.clearSessionState();
        }
      }
    }
  }

  /**
   * Log in user using the backend Unified Login API
   */
  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${environment.apiUrl}/api/auth/login`, dto).pipe(
      tap((res) => {
        this.tokenService.setToken(res.token);
        const decoded = this.tokenService.decodeToken(res.token);
        if (decoded) {
          const userSession: UserSession = {
            token: res.token,
            userId: res.userId,
            tenantId: res.tenantId,
            email: res.email,
            role: res.role,
            decodedToken: decoded
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
    // Attempt to call backend logout endpoint (optional, fire-and-forget)
    if (this.isAuthenticated()) {
      this.http.post(`${environment.apiUrl}/api/auth/logout`, {}).subscribe({
        next: () => { },
        error: () => { } // Suppress backend errors to ensure client-side logout completes
      });
    }

    this.clearSessionState();

    // Redirect to login page
    const queryParams = reason ? { reason } : {};
    this.router.navigate(['/login'], { queryParams });
  }

  /**
   * Forces client-side session cleanup due to expiry or interceptor actions
   */
  clearSessionState(): void {
    this.tokenService.clearToken();
    this.currentUser.set(null);

    // Clear other transient UI state from sessionStorage
    try {
      sessionStorage.clear();
    } catch { }
  }
}
