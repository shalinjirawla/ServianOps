import { Injectable, inject, computed } from '@angular/core';
import { AuthService } from './auth.service';

export enum UserRoleType {
  SuperAdmin = 'SuperAdmin',
  Administrator = 'Administrator',
  User = 'User'
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly authService = inject(AuthService);

  // Expose roles using signals
  readonly currentRole = computed(() => this.authService.userRole());

  readonly isSuperAdmin = computed(() => this.currentRole() === UserRoleType.SuperAdmin);
  readonly isAdministrator = computed(() => this.currentRole() === UserRoleType.Administrator);

  /**
   * Evaluates if the current user has any of the specified roles
   */
  hasAnyRole(roles: string[]): boolean {
    const activeRole = this.currentRole();
    if (!activeRole) return false;
    return roles.includes(activeRole);
  }
}
