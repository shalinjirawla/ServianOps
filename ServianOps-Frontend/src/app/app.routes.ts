import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

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
                title: 'Tenants — ServianOps',
            },
            {
                path: 'users',
                loadComponent: () => import('./pages/users/users.component').then((m) => m.UsersComponent),
                title: 'Users — ServianOps',
            },
            {
                path: 'clients',
                loadComponent: () => import('./pages/clients/clients.component').then((m) => m.ClientsComponent),
                title: 'Clients — ServianOps',
            },
            {
                path: 'sites',
                loadComponent: () => import('./pages/sites/sites.component').then((m) => m.SitesComponent),
                title: 'Sites — ServianOps',
            },
            {
                path: 'jobs',
                loadComponent: () => import('./pages/jobs/jobs.component').then((m) => m.JobsComponent),
                title: 'Jobs — ServianOps',
            },
            {
                path: 'planner',
                loadComponent: () => import('./pages/planner/planner.component').then((m) => m.PlannerComponent),
                title: 'Planner — ServianOps',
            },
            {
                path: 'engineers',
                loadComponent: () => import('./pages/engineers/engineers.component').then((m) => m.EngineersComponent),
                title: 'Engineers — ServianOps',
            },
            {
                path: 'quotes',
                loadComponent: () => import('./pages/quotes/quotes.component').then((m) => m.QuotesComponent),
                title: 'Quotes — ServianOps',
            },
            {
                path: 'invoices',
                loadComponent: () => import('./pages/invoices/invoices.component').then((m) => m.InvoicesComponent),
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
