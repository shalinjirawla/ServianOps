import { CanMatchFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanMatchFn = (route, segments) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Read required roles defined in route configuration
  const allowedRoles = route.data?.['roles'] as string[];

  if (!allowedRoles || allowedRoles.length === 0) {
    return true;
  }

  const role = authService.userRole();
  if (role && allowedRoles.includes(role)) {
    return true;
  }

  // User does not possess authorization. Deny matching and redirect to home.
  console.warn(`Access Denied: Required roles: [${allowedRoles.join(', ')}]. Current role: ${role}`);
  router.navigate(['/dashboard']);
  return false;
};
