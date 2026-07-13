import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { permissionGuard } from './core/guards/permission.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    {
        path: 'auth/login',
        loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent),
        title: 'Sign in — ServianOps',
    },
    {
        path: 'auth/forgot-password',
        loadComponent: () => import('./pages/forgot-password/forgot-password.component').then((m) => m.ForgotPasswordComponent),
        title: 'Reset password — ServianOps',
    },
    {
        path: '',
        loadComponent: () => import('./layout/app-layout/app-layout.component').then((m) => m.AppLayoutComponent),
        canActivate: [authGuard],
        children: [
            {
                path: 'dashboard',
                loadComponent: () => import('./pages/dashboard/dashboard.component').then((m) => m.DashboardComponent),
                title: 'Dashboard — ServianOps',
            },
            {
                path: 'tenants',
                loadComponent: () => import('./pages/tenants/tenants.component').then((m) => m.TenantsComponent),
                canActivate: [() => permissionGuard('Pages.Tenants.View')],
                title: 'Tenants — ServianOps',
            },
            {
                path: 'users',
                loadComponent: () => import('./pages/users/users.component').then((m) => m.UsersComponent),
                canActivate: [() => permissionGuard('Pages.Users.View')],
                title: 'Users — ServianOps',
            },
            {
                path: 'clients',
                loadComponent: () => import('./pages/clients/clients.component').then((m) => m.ClientsComponent),
                canActivate: [() => permissionGuard('Pages.Customers.View')],
                title: 'Clients — ServianOps',
            },
            {
                path: 'customer-users',
                loadComponent: () => import('./pages/customer-users/customer-users.component').then((m) => m.CustomerUsersComponent),
                canActivate: [() => permissionGuard('Pages.CustomerUsers.View')],
                title: 'Customer Users — ServianOps',
            },
            {
                path: 'portfolios',
                loadComponent: () => import('./pages/portfolios/portfolios.component').then((m) => m.PortfoliosComponent),
                canActivate: [() => permissionGuard('Pages.Portfolios.View')],
                title: 'Portfolios — ServianOps',
            },
            {
                path: 'sites',
                loadComponent: () => import('./pages/sites/sites.component').then((m) => m.SitesComponent),
                canActivate: [() => permissionGuard('Pages.Properties.View')],
                title: 'Sites — ServianOps',
            },
            {
                path: 'properties-workspace',
                loadComponent: () => import('./pages/properties-workspace/properties-workspace.component').then((m) => m.PropertiesWorkspaceComponent),
                canActivate: [() => permissionGuard('Pages.Properties.View')],
                title: 'Properties & Assets Workspace — ServianOps',
            },
            {
                path: 'jobs',
                loadComponent: () => import('./pages/jobs/jobs.component').then((m) => m.JobsComponent),
                canActivate: [() => permissionGuard('Pages.Jobs.View')],
                title: 'Jobs — ServianOps',
            },
            {
                path: 'visits',
                loadComponent: () => import('./pages/visits/visits.component').then((m) => m.VisitsComponent),
                canActivate: [() => permissionGuard('Pages.Visits.View')],
                title: 'Visits — ServianOps',
            },
            {
                path: 'planner',
                loadComponent: () => import('./pages/planner/planner.component').then((m) => m.PlannerComponent),
                canActivate: [() => permissionGuard('Pages.Jobs.View')],
                title: 'Planner — ServianOps',
            },
            {
                path: 'engineers',
                loadComponent: () => import('./pages/engineers/engineers.component').then((m) => m.EngineersComponent),
                canActivate: [() => permissionGuard('Pages.Engineers.View')],
                title: 'Engineers — ServianOps',
            },
            {
                path: 'quotes',
                loadComponent: () => import('./pages/quotes/quotes.component').then((m) => m.QuotesComponent),
                canActivate: [() => permissionGuard('Pages.Quotes.View')],
                title: 'Quotes — ServianOps',
            },
            {
                path: 'invoices',
                loadComponent: () => import('./pages/invoices/invoices.component').then((m) => m.InvoicesComponent),
                canActivate: [() => permissionGuard('Pages.Invoices.View')],
                title: 'Invoices — ServianOps',
            },
            {
                path: 'settings',
                loadComponent: () => import('./pages/settings/settings.component').then((m) => m.SettingsComponent),
                title: 'Settings — ServianOps',
            },
        ],
    },
    {
        path: '**',
        loadComponent: () => import('./pages/not-found/not-found.component').then((m) => m.NotFoundComponent),
    },
];
