import { Injectable, inject, effect, DestroyRef } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private expirationTimer: any = null;

  constructor() {
    // Monitor the user state signal. If user changes (login/logout/refresh), reset timer.
    effect(() => {
      const user = this.authService.currentUser();
      this.clearTimer();

      if (user && user.decodedToken.exp) {
        this.scheduleExpirationCheck(user.decodedToken.exp);
      }
    });

    // Handle service cleanup to prevent memory leaks
    this.destroyRef.onDestroy(() => {
      this.clearTimer();
    });
  }

  /**
   * Schedule the logout action to trigger exactly when the JWT expires
   */
  private scheduleExpirationCheck(expSeconds: number): void {
    const currentTimeMs = Date.now();
    const expTimeMs = expSeconds * 1000;
    const timeRemainingMs = expTimeMs - currentTimeMs;

    if (timeRemainingMs <= 0) {
      // Token is already expired, logout immediately
      this.triggerSessionExpiry();
    } else {
      // Schedule logout at exact token expiration
      this.expirationTimer = setTimeout(() => {
        this.triggerSessionExpiry();
      }, timeRemainingMs);
    }
  }

  /**
   * Logs out user with explicit session expiration reason
   */
  private triggerSessionExpiry(): void {
    this.authService.logout('expired');
  }

  /**
   * Clears any active timer
   */
  private clearTimer(): void {
    if (this.expirationTimer) {
      clearTimeout(this.expirationTimer);
      this.expirationTimer = null;
    }
  }
}
