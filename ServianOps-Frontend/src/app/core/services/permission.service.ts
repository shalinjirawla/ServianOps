import { Injectable, inject, computed } from '@angular/core';
import { RoleService, UserRoleType } from './role.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private readonly roleService = inject(RoleService);

  // Centralized client-side mapping of Role -> Permissions.
  // In the future, this can merge permissions claims directly if returned in JWT.
  private readonly rolePermissionMap: Record<string, string[]> = {
    [UserRoleType.SuperAdmin]: [
      'dashboard',
      'tenants',
      'plans',
      'profile',
      'settings'
    ],
    [UserRoleType.Administrator]: [
      'dashboard',
      'customers',
      'jobs',
      'invoices',
      'reports',
      'engineers',
      'stock',
      'settings'
    ]
  };

  /**
   * Signal exposing current user permissions list
   */
  readonly userPermissions = computed(() => {
    const role = this.roleService.currentRole();
    if (!role) return [];
    return this.rolePermissionMap[role] || [];
  });

  /**
   * Checks if current user has a specific permission
   */
  hasPermission(permission: string): boolean {
    return this.userPermissions().includes(permission);
  }

  /**
   * Checks if current user has any of the listed permissions
   */
  hasAnyPermission(permissions: string[]): boolean {
    const userPerms = this.userPermissions();
    return permissions.some((p) => userPerms.includes(p));
  }
}
