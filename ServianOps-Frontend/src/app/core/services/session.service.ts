import { Injectable, inject, effect, DestroyRef } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  
  private expirationTimer: any = null;
  private refreshTimer: any = null;
  private inactivityInterval: any = null;
  private lastActivityTime = Date.now();
  private activityListeners: (() => void)[] = [];

  constructor() {
    this.initActivityTracking();
    this.startInactivityCheck();

    // Monitor the user state signal. If user changes (login/logout/refresh), reset timers.
    effect(() => {
      const user = this.authService.currentUser();
      this.clearSessionTimers();

      if (user && user.decodedToken.exp) {
        this.scheduleSessionTimers(user.decodedToken.exp);
      }
    });

    // Handle service cleanup to prevent memory leaks
    this.destroyRef.onDestroy(() => {
      this.clearSessionTimers();
      if (this.inactivityInterval) {
        clearInterval(this.inactivityInterval);
        this.inactivityInterval = null;
      }
      this.cleanupActivityTracking();
    });
  }

  /**
   * Initialize event listeners to monitor user interaction
   */
  private initActivityTracking(): void {
    if (typeof window === 'undefined') return;

    const events = ['mousemove', 'mousedown', 'keypress', 'scroll', 'click', 'touchstart'];
    const updateActivity = () => {
      this.lastActivityTime = Date.now();
    };

    events.forEach(evt => {
      window.addEventListener(evt, updateActivity, { passive: true });
    });

    this.activityListeners.push(() => {
      events.forEach(evt => {
        window.removeEventListener(evt, updateActivity);
      });
    });
  }

  /**
   * Cleanup activity tracking event listeners
   */
  private cleanupActivityTracking(): void {
    this.activityListeners.forEach(cleanup => cleanup());
    this.activityListeners = [];
  }

  /**
   * Check for inactivity timeout periodically (every 10 seconds)
   */
  private startInactivityCheck(): void {
    if (typeof window === 'undefined') return;

    const CHECK_INTERVAL = 10 * 1000; // 10 seconds
    const INACTIVITY_LIMIT = 15 * 60 * 1000; // 15 minutes

    this.inactivityInterval = setInterval(() => {
      if (this.authService.isAuthenticated()) {
        const idleDuration = Date.now() - this.lastActivityTime;
        if (idleDuration >= INACTIVITY_LIMIT) {
          console.warn('SessionService: Session timed out due to user inactivity.');
          this.clearSessionTimers();
          this.authService.logout('inactive');
        }
      }
    }, CHECK_INTERVAL);
  }

  /**
   * Schedules expiration logout and auto token refresh
   */
  private scheduleSessionTimers(expSeconds: number): void {
    const currentTimeMs = Date.now();
    const expTimeMs = expSeconds * 1000;
    const timeRemainingMs = expTimeMs - currentTimeMs;

    if (timeRemainingMs <= 0) {
      this.triggerSessionExpiry();
      return;
    }

    // 1. Schedule Hard Expiry Logout
    this.expirationTimer = setTimeout(() => {
      this.triggerSessionExpiry();
    }, timeRemainingMs);

    // 2. Schedule Sliding Token Refresh (5 minutes before expiration)
    const REFRESH_BUFFER = 5 * 60 * 1000; // 5 minutes
    const timeToRefreshMs = timeRemainingMs - REFRESH_BUFFER;

    if (timeToRefreshMs > 0) {
      this.refreshTimer = setTimeout(() => {
        this.attemptTokenRefresh();
      }, timeToRefreshMs);
    } else {
      // Token expires in less than 5 minutes - attempt immediate refresh if active
      this.attemptTokenRefresh();
    }
  }

  /**
   * Attempt token refresh if the user has had recent activity
   */
  private attemptTokenRefresh(): void {
    const idleDuration = Date.now() - this.lastActivityTime;
    const REFRESH_INACTIVITY_LIMIT = 10 * 60 * 1000; // 10 minutes

    if (idleDuration < REFRESH_INACTIVITY_LIMIT) {
      console.log('SessionService: User active near expiration. Requesting silent token refresh...');
      this.authService.refreshToken().subscribe({
        next: () => {
          console.log('SessionService: Token refreshed successfully.');
        },
        error: (err) => {
          console.error('SessionService: Token refresh failed. Fallback to natural expiration.', err);
        }
      });
    } else {
      console.log('SessionService: User idle. Skipping refresh, allowing token to expire naturally.');
    }
  }

  /**
   * Logs out user with explicit session expiration reason
   */
  private triggerSessionExpiry(): void {
    this.authService.logout('expired');
  }

  /**
   * Clears any active sliding/expiration timers
   */
  private clearSessionTimers(): void {
    if (this.expirationTimer) {
      clearTimeout(this.expirationTimer);
      this.expirationTimer = null;
    }
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }
}
