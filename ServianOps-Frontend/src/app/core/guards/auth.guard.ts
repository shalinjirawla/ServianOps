import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.isAuthenticated()) {
    // Role-based landing page redirection for SuperAdmin users visiting root/dashboard
    const role = authService.userRole();
    if (role === 'SuperAdmin' && (state.url === '/' || state.url === '/dashboard' || state.url === '')) {
      router.navigate(['/tenants']);
      return false;
    }
    return true;
  }

  // Redirect to login page and keep returnUrl context
  router.navigate(['/auth/login'], {
    queryParams: { returnUrl: state.url }
  });
  return false;
};
