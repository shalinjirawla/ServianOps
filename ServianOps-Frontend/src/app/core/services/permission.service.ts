import { Injectable, inject, computed } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private readonly authService = inject(AuthService);

  // Centralized client-side mapping of Role -> Permissions matching routing guards.
  private readonly rolePermissionMap: Record<string, string[]> = {
    'SuperAdmin': [
      'Pages.Tenants.View'
    ],
    'Admin': [
      'Pages.Users.View',
      'Pages.Customers.View',
      'Pages.Properties.View',
      'Pages.Jobs.View',
      'Pages.Engineers.View',
      'Pages.Quotes.View',
      'Pages.Invoices.View'
    ],
    'Administrator': [
      'Pages.Users.View',
      'Pages.Customers.View',
      'Pages.Properties.View',
      'Pages.Jobs.View',
      'Pages.Engineers.View',
      'Pages.Quotes.View',
      'Pages.Invoices.View'
    ]
  };

  /**
   * Signal exposing current user permissions list
   */
  readonly userPermissions = computed(() => {
    const role = this.authService.userRole();
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
