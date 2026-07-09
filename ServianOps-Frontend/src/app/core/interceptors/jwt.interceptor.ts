import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';

// Lock state to prevent redundant logouts when multiple concurrent requests trigger 401s
let isLoggingOut = false;

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  
  const token = tokenService.getToken();
  let modifiedReq = req;

  // 1. Inject Bearer Token if present
  if (token) {
    modifiedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // 2. Process request and catch security errors
  return next(modifiedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error instanceof HttpErrorResponse) {
        switch (error.status) {
          case 401:
            // Token is invalid or expired
            if (!isLoggingOut) {
              isLoggingOut = true;
              
              // Extract expiration status
              const isExpired = tokenService.isCurrentTokenExpired();
              authService.clearSessionState();
              
              // Gracefully reset locking state after routing task completes
              setTimeout(() => {
                isLoggingOut = false;
                authService.logout(isExpired ? 'expired' : 'unauthorized');
              }, 100);
            }
            break;

          case 403:
            // Forbidden access - User has valid session but is unauthorized for this action
            console.error('Security Alert: Access Forbidden (403)', error.url);
            break;

          case 500:
            console.error('Server side error (500)', error.message);
            break;

          case 0:
            // Network connection errors
            console.error('Connection failure - check your network connection', error.message);
            break;
        }
      }
      return throwError(() => error);
    })
  );
};
