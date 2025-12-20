import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './guards';
import { permissionGuard } from './guards';

export const routes: Routes = [
  // Public routes
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'forgot-password',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
  },

  // Protected routes
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./components/layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },

      // Account Management
      {
        path: 'accounts',
        canActivate: [permissionGuard(['account:read'])],
        loadComponent: () => import('./pages/accounts/account-list.component').then(m => m.AccountListComponent)
      },
      {
        path: 'accounts/new',
        canActivate: [permissionGuard(['account:write'])],
        loadComponent: () => import('./pages/accounts/account-form.component').then(m => m.AccountFormComponent)
      },
      {
        path: 'accounts/:id',
        canActivate: [permissionGuard(['account:write'])],
        loadComponent: () => import('./pages/accounts/account-form.component').then(m => m.AccountFormComponent)
      },

      // User Management
      {
        path: 'users',
        canActivate: [permissionGuard(['user:read'])],
        loadComponent: () => import('./pages/users/user-list.component').then(m => m.UserListComponent)
      },
      {
        path: 'users/invite',
        canActivate: [permissionGuard(['user:create'])],
        loadComponent: () => import('./pages/users/user-form.component').then(m => m.UserFormComponent)
      },
      {
        path: 'users/:id',
        canActivate: [permissionGuard(['user:write'])],
        loadComponent: () => import('./pages/users/user-form.component').then(m => m.UserFormComponent)
      },

      // Role Management
      {
        path: 'roles',
        canActivate: [permissionGuard(['role:read'])],
        loadComponent: () => import('./pages/roles/role-list.component').then(m => m.RoleListComponent)
      },
      {
        path: 'roles/new',
        canActivate: [permissionGuard(['role:create'])],
        loadComponent: () => import('./pages/roles/role-form.component').then(m => m.RoleFormComponent)
      },
      {
        path: 'roles/:id',
        canActivate: [permissionGuard(['role:write'])],
        loadComponent: () => import('./pages/roles/role-form.component').then(m => m.RoleFormComponent)
      },

      // Profile Management
      {
        path: 'profiles',
        canActivate: [permissionGuard(['profile:read'])],
        loadComponent: () => import('./pages/profiles/profile-list.component').then(m => m.ProfileListComponent)
      },
      {
        path: 'profiles/new',
        canActivate: [permissionGuard(['profile:create'])],
        loadComponent: () => import('./pages/profiles/profile-form.component').then(m => m.ProfileFormComponent)
      },
      {
        path: 'profiles/:id',
        canActivate: [permissionGuard(['profile:write'])],
        loadComponent: () => import('./pages/profiles/profile-form.component').then(m => m.ProfileFormComponent)
      },

      // Content Management
      {
        path: 'content',
        canActivate: [permissionGuard(['content:read'])],
        loadComponent: () => import('./pages/content/content-list.component').then(m => m.ContentListComponent)
      },
      {
        path: 'content/new',
        canActivate: [permissionGuard(['content:create'])],
        loadComponent: () => import('./pages/content/content-form.component').then(m => m.ContentFormComponent)
      },
      {
        path: 'content/:id',
        canActivate: [permissionGuard(['content:write'])],
        loadComponent: () => import('./pages/content/content-form.component').then(m => m.ContentFormComponent)
      },

      // Media Management
      {
        path: 'media',
        canActivate: [permissionGuard(['media:read'])],
        loadComponent: () => import('./pages/media/media-list.component').then(m => m.MediaListComponent)
      },

      // Workflow Management
      {
        path: 'workflows',
        canActivate: [permissionGuard(['workflow:read'])],
        loadComponent: () => import('./pages/workflows/workflow-list.component').then(m => m.WorkflowListComponent)
      },

      // API Key Management
      {
        path: 'api-keys',
        canActivate: [permissionGuard(['apikey:read'])],
        loadComponent: () => import('./pages/api-keys/api-key-list.component').then(m => m.ApiKeyListComponent)
      },

      // Webhook Management
      {
        path: 'webhooks',
        canActivate: [permissionGuard(['webhook:read'])],
        loadComponent: () => import('./pages/webhooks/webhook-list.component').then(m => m.WebhookListComponent)
      },
      {
        path: 'webhooks/new',
        canActivate: [permissionGuard(['webhook:create'])],
        loadComponent: () => import('./pages/webhooks/webhook-form.component').then(m => m.WebhookFormComponent)
      },
      {
        path: 'webhooks/:id',
        canActivate: [permissionGuard(['webhook:write'])],
        loadComponent: () => import('./pages/webhooks/webhook-form.component').then(m => m.WebhookFormComponent)
      },

      // Audit Logs
      {
        path: 'audit',
        canActivate: [permissionGuard(['audit:read'])],
        loadComponent: () => import('./pages/audit/audit-log.component').then(m => m.AuditLogComponent)
      },

      // System Management
      {
        path: 'system',
        canActivate: [permissionGuard(['system:read'])],
        loadComponent: () => import('./pages/system/system.component').then(m => m.SystemComponent)
      },

      // Settings
      {
        path: 'settings',
        loadComponent: () => import('./pages/settings/settings.component').then(m => m.SettingsComponent)
      }
    ]
  },

  // Fallback
  { path: '**', redirectTo: 'dashboard' }
];
