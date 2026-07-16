import { HttpInterceptorFn, HttpErrorResponse, HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take, tap, finalize } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';
import { environment } from '../../../environments/environment';
import { AuthResponseDto } from '../models/auth.models';

let isRefreshing = false;
let refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const toastService = inject(ToastService);
  const http = inject(HttpClient);

  // 1. Always inject withCredentials for our API to send the HttpOnly cookies
  let modifiedReq = req;
  const isApiUrl = req.url.startsWith(environment.apiUrl) || req.url.startsWith('/api/');
  if (isApiUrl) {
    modifiedReq = req.clone({
      withCredentials: true
    });
  }

  // 2. Process request and catch security errors
  return next(modifiedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error instanceof HttpErrorResponse) {
        if (error.status === 401 && isApiUrl && !req.url.includes('/auth/login') && !req.url.includes('/auth/refresh')) {
          return handle401Error(modifiedReq, next, authService, http);
        }
        
        switch (error.status) {
          case 403:
            console.error('Security Alert: Access Forbidden (403)', error.url);
            toastService.error('Access Denied: You do not have permissions to perform this action.');
            break;
          case 500:
            console.error('Server side error (500)', error.message);
            toastService.error('An unexpected server error occurred. Please try again later.');
            break;
          case 0:
            console.error('Connection failure', error.message);
            toastService.error('Network error: Unable to connect to the server. Please check your connection.');
            break;
        }
      }
      return throwError(() => error);
    })
  );
};

const handle401Error = (req: any, next: any, authService: AuthService, http: HttpClient): import('rxjs').Observable<import('@angular/common/http').HttpEvent<unknown>> => {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    return http.post<{ data: AuthResponseDto }>(`${environment.apiUrl}/api/auth/refresh`, {}, { withCredentials: true }).pipe(
      switchMap((res) => {
        isRefreshing = false;
        
        if (res && res.data) {
          refreshTokenSubject.next(res.data);
          // Re-attempt the failed request
          return next(req) as import('rxjs').Observable<import('@angular/common/http').HttpEvent<unknown>>;
        } else {
          authService.logout('expired');
          return throwError(() => new Error('Refresh failed')) as import('rxjs').Observable<import('@angular/common/http').HttpEvent<unknown>>;
        }
      }),
      catchError((err) => {
        isRefreshing = false;
        authService.logout('expired');
        return throwError(() => err) as import('rxjs').Observable<import('@angular/common/http').HttpEvent<unknown>>;
      }),
      finalize(() => {
         isRefreshing = false;
      })
    );
  } else {
    // Wait for the refresh to complete
    return refreshTokenSubject.pipe(
      filter(result => result !== null),
      take(1),
      switchMap(() => next(req) as import('rxjs').Observable<import('@angular/common/http').HttpEvent<unknown>>)
    ) as import('rxjs').Observable<import('@angular/common/http').HttpEvent<unknown>>;
  }
};
