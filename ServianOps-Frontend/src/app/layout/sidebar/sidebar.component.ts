import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { IconComponent } from '../../shared/icon/icon.component';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../shared/toast/toast.service';
import { SidebarService } from './sidebar.service';

interface NavItem {
  title: string;
  url: string;
  icon: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, IconComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  readonly isSuperAdmin = computed(() => this.auth.userRole() === 'SuperAdmin');

  constructor(
    private router: Router,
    private auth: AuthService,
    private toast: ToastService,
    public sidebarSvc: SidebarService
  ) {}

  readonly filteredMain = computed(() => {
    if (this.isSuperAdmin()) {
      return [
        { title: 'Dashboard', url: '/dashboard', icon: 'layout-dashboard' },
        { title: 'Tenants', url: '/tenants', icon: 'server' },
      ];
    } else {
      return [
        { title: 'Dashboard', url: '/dashboard', icon: 'layout-dashboard' },
        { title: 'Clients', url: '/clients', icon: 'users' },
        { title: 'Sites', url: '/sites', icon: 'building-2' },
        { title: 'Jobs', url: '/jobs', icon: 'briefcase' },
        { title: 'Planner', url: '/planner', icon: 'calendar-clock' },
        { title: 'Engineers', url: '/engineers', icon: 'hard-hat' },
        { title: 'Users', url: '/users', icon: 'user-2' },
      ];
    }
  });

  readonly filteredCommercial = computed(() => {
    if (this.isSuperAdmin()) {
      return [];
    } else {
      return [
        { title: 'Quotes', url: '/quotes', icon: 'file-text' },
        { title: 'Invoices', url: '/invoices', icon: 'receipt' },
      ];
    }
  });

  get main(): NavItem[] {
    return this.filteredMain();
  }

  get commercial(): NavItem[] {
    return this.filteredCommercial();
  }

  isActive(url: string): boolean {
    const path = this.router.url;
    return path === url || path.startsWith(url + '/');
  }

  handleSignOut() {
    this.auth.logout();
    this.toast.success('Signed out');
  }
}
