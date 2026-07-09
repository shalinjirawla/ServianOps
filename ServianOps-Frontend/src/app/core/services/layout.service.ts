import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  // Sidebar expanded/collapsed states (desktop view)
  readonly isSidebarCollapsed = signal(false);

  // Mobile menu drawer open/closed states
  readonly isMobileOpen = signal(false);

  // Dynamic header page title
  readonly pageTitle = signal('Dashboard');

  /**
   * Toggle expanded/collapsed state of the main navigation sidebar
   */
  toggleSidebar(): void {
    this.isSidebarCollapsed.update((val) => !val);
  }

  /**
   * Toggle open/closed state of the mobile menu drawer
   */
  toggleMobileMenu(): void {
    this.isMobileOpen.update((val) => !val);
  }

  /**
   * Closes mobile menu drawer explicitly (e.g. on navigation)
   */
  closeMobileMenu(): void {
    this.isMobileOpen.set(false);
  }

  /**
   * Sets dynamic page title
   */
  setPageTitle(title: string): void {
    this.pageTitle.set(title);
  }
}
