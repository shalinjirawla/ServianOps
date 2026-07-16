import { Injectable, inject, effect, DestroyRef } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly authService = inject(AuthService);

  constructor() {
    // Session expiration is now handled by HttpOnly cookies and the JwtInterceptor.
    // When a 401 occurs, the interceptor will attempt to refresh the token, 
    // and if that fails, it will call authService.logout('expired').
  }
}
