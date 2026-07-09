import { CanMatchFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { RoleService } from '../services/role.service';

export const roleGuard: CanMatchFn = (route, segments) => {
  const roleService = inject(RoleService);
  const router = inject(Router);

  // Read required roles defined in route configuration
  const allowedRoles = route.data?.['roles'] as string[];

  if (!allowedRoles || allowedRoles.length === 0) {
    return true;
  }

  if (roleService.hasAnyRole(allowedRoles)) {
    return true;
  }

  // User does not possess authorization. Deny matching and redirect to home.
  console.warn(`Access Denied: Required roles: [${allowedRoles.join(', ')}]. Current role: ${roleService.currentRole()}`);
  router.navigate(['/dashboard']);
  return false;
};
