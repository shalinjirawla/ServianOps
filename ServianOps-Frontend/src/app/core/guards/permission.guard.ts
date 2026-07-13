import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { PermissionService } from '../services/permission.service';

export const permissionGuard = (permission: string): CanActivateFn => {
  return (route, state) => {
    const permissionService = inject(PermissionService);
    const router = inject(Router);

    if (permissionService.hasPermission(permission)) {
      return true;
    }

    console.warn(`Access Denied: Required permission '${permission}' not found.`);
    router.navigate(['/dashboard']);
    return false;
  };
};
