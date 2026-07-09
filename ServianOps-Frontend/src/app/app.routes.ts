import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // 1. Guest Routes (AuthLayout)
  {
    path: '',
    component: AuthLayoutComponent,
    canActivate: [guestGuard],
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login.component').then((c) => c.LoginComponent)
      }
    ]
  },
  // 2. Protected Workspace Routes (AdminLayout)
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((c) => c.DashboardComponent)
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/profile.component').then((c) => c.ProfileComponent),
        canMatch: [roleGuard],
        data: { roles: ['SuperAdmin', 'Administrator'] }
      },
      // SuperAdmin Restricted Routes
      {
        path: 'super-admin/tenants',
        loadComponent: () =>
          import('./features/super-admin/tenants/tenants.component').then(
            (c) => c.TenantsComponent
          ),
        canMatch: [roleGuard],
        data: { roles: ['SuperAdmin'] }
      },
      {
        path: 'super-admin/plans',
        loadComponent: () =>
          import('./features/super-admin/plans/plans.component').then(
            (c) => c.PlansComponent
          ),
        canMatch: [roleGuard],
        data: { roles: ['SuperAdmin'] }
      },
      // Tenant Administrator Restricted Routes
      {
        path: 'tenant/customers',
        loadComponent: () =>
          import('./features/tenant/customers/customers.component').then(
            (c) => c.CustomersComponent
          ),
        canMatch: [roleGuard],
        data: { roles: ['Administrator'] }
      },
      {
        path: 'tenant/jobs',
        loadComponent: () =>
          import('./features/tenant/jobs/jobs.component').then((c) => c.JobsComponent),
        canMatch: [roleGuard],
        data: { roles: ['Administrator'] }
      },
      {
        path: 'tenant/invoices',
        loadComponent: () =>
          import('./features/tenant/invoices/invoices.component').then((c) => c.InvoicesComponent),
        canMatch: [roleGuard],
        data: { roles: ['Administrator'] }
      },
      {
        path: 'tenant/reports',
        loadComponent: () =>
          import('./features/tenant/reports/reports.component').then((c) => c.ReportsComponent),
        canMatch: [roleGuard],
        data: { roles: ['Administrator'] }
      },
      {
        path: 'tenant/engineers',
        loadComponent: () =>
          import('./features/tenant/engineers/engineers.component').then(
            (c) => c.EngineersComponent
          ),
        canMatch: [roleGuard],
        data: { roles: ['Administrator'] }
      },
      {
        path: 'tenant/stock',
        loadComponent: () =>
          import('./features/tenant/stock/stock.component').then((c) => c.StockComponent),
        canMatch: [roleGuard],
        data: { roles: ['Administrator'] }
      },
      // Shared Authorized Settings Route
      {
        path: 'settings',
        loadComponent: () =>
          import('./features/settings/settings.component').then((c) => c.SettingsComponent),
        canMatch: [roleGuard],
        data: { roles: ['SuperAdmin', 'Administrator'] }
      }
    ]
  },
  // 3. Fallback Route
  { path: '**', redirectTo: 'dashboard' }
];
