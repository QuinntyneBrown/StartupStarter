import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './guards';

export const routes: Routes = [
  // Auth routes (public)
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/auth/login').then(m => m.Login)
  },
  {
    path: 'forgot-password',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/auth/forgot-password').then(m => m.ForgotPassword)
  },
  {
    path: 'reset-password',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/auth/reset-password').then(m => m.ResetPassword)
  },

  // Protected routes with admin layout
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./components/layout/admin-layout').then(m => m.AdminLayout),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard)
      },

      // Account routes
      {
        path: 'accounts',
        loadComponent: () => import('./pages/accounts/account-list').then(m => m.AccountList)
      },
      {
        path: 'accounts/new',
        loadComponent: () => import('./pages/accounts/account-form').then(m => m.AccountForm)
      },
      {
        path: 'accounts/:id',
        loadComponent: () => import('./pages/accounts/account-form').then(m => m.AccountForm)
      },
      {
        path: 'accounts/:id/edit',
        loadComponent: () => import('./pages/accounts/account-form').then(m => m.AccountForm)
      },

      // User routes
      {
        path: 'users',
        loadComponent: () => import('./pages/users/user-list').then(m => m.UserList)
      },

      // Role routes
      {
        path: 'roles',
        loadComponent: () => import('./pages/roles/role-list').then(m => m.RoleList)
      },

      // Content routes
      {
        path: 'content',
        loadComponent: () => import('./pages/content/content-list').then(m => m.ContentList)
      },

      // Profile management routes
      {
        path: 'profiles',
        loadComponent: () => import('./pages/profiles/profile-list').then(m => m.ProfileList)
      },

      // Media routes
      {
        path: 'media',
        loadComponent: () => import('./pages/media/media-list').then(m => m.MediaList)
      },

      // Workflow routes
      {
        path: 'workflows',
        loadComponent: () => import('./pages/workflows/workflow-list').then(m => m.WorkflowList)
      },

      // API routes
      {
        path: 'api-keys',
        loadComponent: () => import('./pages/api/api-key-list').then(m => m.ApiKeyList)
      },
      {
        path: 'webhooks',
        loadComponent: () => import('./pages/api/webhook-list').then(m => m.WebhookList)
      },

      // Audit routes
      {
        path: 'audit',
        loadComponent: () => import('./pages/audit/audit-list').then(m => m.AuditList)
      },

      // System routes
      {
        path: 'system',
        loadComponent: () => import('./pages/system/system-settings').then(m => m.SystemSettings)
      },

      // User settings routes
      {
        path: 'settings',
        loadComponent: () => import('./pages/settings/user-settings').then(m => m.UserSettings)
      },

      // Current user profile
      {
        path: 'profile',
        loadComponent: () => import('./pages/profile/user-profile').then(m => m.UserProfile)
      }
    ]
  },

  // Fallback
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
