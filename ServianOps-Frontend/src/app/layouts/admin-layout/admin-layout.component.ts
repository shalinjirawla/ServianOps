import { Component, inject, signal, computed, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { LayoutService } from '../../core/services/layout.service';
import { PermissionService } from '../../core/services/permission.service';
import { ThemeService } from '../../core/services/theme.service';
import { ToastService } from '../../core/services/toast.service';

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  permission: string;
}

interface SearchItem {
  title: string;
  category: 'Pages' | 'Jobs' | 'Customers';
  description: string;
  route: string;
}

interface NotificationItem {
  id: string;
  title: string;
  message: string;
  time: string;
  type: 'info' | 'success' | 'warning';
  unread: boolean;
}

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {
  readonly authService = inject(AuthService);
  readonly layoutService = inject(LayoutService);
  readonly themeService = inject(ThemeService);
  private readonly permissionService = inject(PermissionService);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);

  // Profile dropdown visibility
  readonly isProfileDropdownOpen = signal(false);

  // Notifications overlay visibility
  readonly isNotificationsOpen = signal(false);

  // Search overlay command palette visibility
  readonly isSearchOpen = signal(false);
  readonly searchQuery = signal('');
  readonly selectedSearchIndex = signal(0);
  readonly activeSearchCategory = signal<'All' | 'Pages' | 'Jobs' | 'Customers'>('All');

  // Master menu items definition list mapping to respective feature routes & permissions
  private readonly masterMenuItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'home', route: '/dashboard', permission: 'dashboard' },
    { label: 'Tenants', icon: 'building-office-2', route: '/super-admin/tenants', permission: 'tenants' },
    { label: 'Plans', icon: 'credit-card', route: '/super-admin/plans', permission: 'plans' },
    { label: 'Customers', icon: 'users', route: '/tenant/customers', permission: 'customers' },
    { label: 'Jobs', icon: 'wrench-screwdriver', route: '/tenant/jobs', permission: 'jobs' },
    { label: 'Invoices', icon: 'document-text', route: '/tenant/invoices', permission: 'invoices' },
    { label: 'Reports', icon: 'presentation-chart-bar', route: '/tenant/reports', permission: 'reports' },
    { label: 'Engineers', icon: 'user-group', route: '/tenant/engineers', permission: 'engineers' },
    { label: 'Stock', icon: 'cube', route: '/tenant/stock', permission: 'stock' },
    { label: 'Settings', icon: 'cog-6-tooth', route: '/settings', permission: 'settings' }
  ];

  // Dynamic filter computing active sidebar menus purely from backend permissions
  readonly menuItems = computed(() => {
    return this.masterMenuItems.filter((item) =>
      this.permissionService.hasPermission(item.permission)
    );
  });

  // Notifications List state
  readonly notifications = signal<NotificationItem[]>([
    { id: 'n1', title: 'New Job Dispatched', message: 'Job #JOB-000184 has been successfully assigned to Mike Johnson.', time: '5m ago', type: 'success', unread: true },
    { id: 'n2', title: 'Invoice Pending Action', message: 'Customer Alpha Corporation has #INV-2026-904 outstanding.', time: '2h ago', type: 'warning', unread: true },
    { id: 'n3', title: 'Stripe Webhook Synced', message: 'ApexField Solutions recurring package payment processed.', time: '4h ago', type: 'info', unread: false },
    { id: 'n4', title: 'Database Backup Complete', message: 'Automated 12-hour PostgreSQL database dump succeeded.', time: '12h ago', type: 'info', unread: false }
  ]);

  readonly unreadNotificationsCount = computed(() => {
    return this.notifications().filter(n => n.unread).length;
  });

  // Global searchable registry index
  private readonly searchRegistry: SearchItem[] = [
    { title: 'Dashboard', category: 'Pages', description: 'Overview statistics, metrics trends, active charts.', route: '/dashboard' },
    { title: 'System Settings', category: 'Pages', description: 'Configurations, general security, profiles settings.', route: '/settings' },
    { title: 'Customers Directory', category: 'Pages', description: 'List of corporate client accounts & depots.', route: '/tenant/customers' },
    { title: 'Jobs Dispatcher', category: 'Pages', description: 'Schedule tasks, track maintenance engineers.', route: '/tenant/jobs' },
    { title: 'ABC Services Ltd', category: 'Customers', description: 'Billing client, London, CUST-001.', route: '/tenant/customers' },
    { title: 'XYZ Industries', category: 'Customers', description: 'Billing client, Manchester, CUST-002.', route: '/tenant/customers' },
    { title: 'Emergency HVAC Maintenance', category: 'Jobs', description: 'Job #JOB-000184 dispatch file - ABC Services.', route: '/tenant/jobs' },
    { title: 'AC Repair Assessment', category: 'Jobs', description: 'Job #JOB-000182 completed report - Mike Johnson.', route: '/tenant/jobs' }
  ];

  // Filtered search list computation
  readonly filteredSearchResults = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const category = this.activeSearchCategory();
    
    let items = this.searchRegistry;
    if (category !== 'All') {
      items = items.filter(i => i.category === category);
    }

    if (!query) return items.slice(0, 5);

    return items.filter(item => 
      item.title.toLowerCase().includes(query) || 
      item.description.toLowerCase().includes(query) ||
      item.category.toLowerCase().includes(query)
    );
  });

  // Hotkey listener for Cmd/Ctrl+K command palette trigger
  @HostListener('window:keydown', ['$event'])
  handleKeyboardShortcuts(event: KeyboardEvent): void {
    if ((event.metaKey || event.ctrlKey) && event.key === 'k') {
      event.preventDefault();
      this.toggleSearchPalette();
    }
    
    if (this.isSearchOpen()) {
      if (event.key === 'Escape') {
        this.closeSearchPalette();
      } else if (event.key === 'ArrowDown') {
        event.preventDefault();
        const results = this.filteredSearchResults();
        this.selectedSearchIndex.update(idx => (idx + 1) % results.length);
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        const results = this.filteredSearchResults();
        this.selectedSearchIndex.update(idx => (idx - 1 + results.length) % results.length);
      } else if (event.key === 'Enter') {
        event.preventDefault();
        const results = this.filteredSearchResults();
        if (results.length > 0) {
          this.navigateToSearchItem(results[this.selectedSearchIndex()]);
        }
      }
    }
  }

  // Dynamic breadcrumb paths resolution based on route history
  readonly breadcrumbs = computed(() => {
    const url = this.router.url.split('?')[0]; // Remove query params
    const segments = url.split('/').filter(s => s !== '');

    if (segments.length === 0) {
      return [{ label: 'Home', url: '/dashboard' }];
    }

    return segments.map((seg, i) => {
      const label = seg.charAt(0).toUpperCase() + seg.slice(1).replace(/-/g, ' ');
      const fullUrl = '/' + segments.slice(0, i + 1).join('/');
      return { label, url: fullUrl };
    });
  });

  toggleProfileDropdown(): void {
    this.isProfileDropdownOpen.update((val) => !val);
  }

  closeProfileDropdown(): void {
    this.isProfileDropdownOpen.set(false);
  }

  toggleNotifications(): void {
    this.isNotificationsOpen.update((val) => !val);
    if (this.isNotificationsOpen()) {
      this.isProfileDropdownOpen.set(false);
      this.isSearchOpen.set(false);
    }
  }

  closeNotifications(): void {
    this.isNotificationsOpen.set(false);
  }

  toggleSearchPalette(): void {
    this.isSearchOpen.update((val) => !val);
    if (this.isSearchOpen()) {
      this.searchQuery.set('');
      this.selectedSearchIndex.set(0);
      this.isProfileDropdownOpen.set(false);
      this.isNotificationsOpen.set(false);
    }
  }

  closeSearchPalette(): void {
    this.isSearchOpen.set(false);
  }

  setSearchCategory(cat: 'All' | 'Pages' | 'Jobs' | 'Customers'): void {
    this.activeSearchCategory.set(cat);
    this.selectedSearchIndex.set(0);
  }

  navigateToSearchItem(item: SearchItem): void {
    this.closeSearchPalette();
    this.router.navigateByUrl(item.route);
    this.toastService.info(`Navigated to: ${item.title}`);
  }

  markAllNotificationsRead(): void {
    this.notifications.update(current => 
      current.map(n => ({ ...n, unread: false }))
    );
    this.toastService.success('All notifications marked as read.');
  }

  clearNotification(id: string): void {
    this.notifications.update(current => 
      current.filter(n => n.id !== id)
    );
    this.toastService.info('Notification cleared.');
  }

  logout(): void {
    this.closeProfileDropdown();
    this.toastService.success('Logged out successfully.');
    this.authService.logout();
  }
}
