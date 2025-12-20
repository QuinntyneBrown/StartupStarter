import { Component, inject, signal, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../services';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  permissions?: string[];
}

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatListModule,
    MatMenuModule,
    MatDividerModule
  ],
  template: `
    <mat-sidenav-container class="admin-layout">
      <mat-sidenav #sidenav mode="side" [opened]="sidenavOpen()" class="admin-layout__sidenav">
        <div class="admin-layout__sidenav-header">
          <h2>StartupStarter</h2>
        </div>
        <mat-nav-list>
          @for (item of filteredNavItems(); track item.route) {
            <a mat-list-item [routerLink]="item.route" routerLinkActive="active">
              <mat-icon matListItemIcon>{{ item.icon }}</mat-icon>
              <span matListItemTitle>{{ item.label }}</span>
            </a>
          }
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content class="admin-layout__content">
        <mat-toolbar color="primary" class="admin-layout__toolbar">
          <button mat-icon-button (click)="toggleSidenav()">
            <mat-icon>menu</mat-icon>
          </button>
          <span class="admin-layout__toolbar-spacer"></span>
          <button mat-icon-button [matMenuTriggerFor]="userMenu">
            <mat-icon>account_circle</mat-icon>
          </button>
          <mat-menu #userMenu="matMenu">
            <div class="admin-layout__user-menu-header">
              <strong>{{ user()?.firstName }} {{ user()?.lastName }}</strong>
              <small>{{ user()?.email }}</small>
            </div>
            <mat-divider></mat-divider>
            <a mat-menu-item routerLink="/settings">
              <mat-icon>settings</mat-icon>
              <span>Settings</span>
            </a>
            <button mat-menu-item (click)="logout()">
              <mat-icon>logout</mat-icon>
              <span>Logout</span>
            </button>
          </mat-menu>
        </mat-toolbar>

        <main class="admin-layout__main">
          <router-outlet></router-outlet>
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .admin-layout {
      height: 100vh;
    }

    .admin-layout__sidenav {
      width: 260px;
      background: #fafafa;
    }

    .admin-layout__sidenav-header {
      padding: 16px 24px;
      border-bottom: 1px solid rgba(0, 0, 0, 0.12);
    }

    .admin-layout__sidenav-header h2 {
      margin: 0;
      font-size: 20px;
      font-weight: 500;
      color: #3f51b5;
    }

    .admin-layout__content {
      display: flex;
      flex-direction: column;
    }

    .admin-layout__toolbar {
      position: sticky;
      top: 0;
      z-index: 100;
    }

    .admin-layout__toolbar-spacer {
      flex: 1;
    }

    .admin-layout__main {
      padding: 24px;
      flex: 1;
      overflow-y: auto;
    }

    .admin-layout__user-menu-header {
      padding: 16px;
      display: flex;
      flex-direction: column;
    }

    .admin-layout__user-menu-header small {
      color: rgba(0, 0, 0, 0.6);
      margin-top: 4px;
    }

    mat-nav-list a.active {
      background-color: rgba(63, 81, 181, 0.1);
      color: #3f51b5;
    }

    mat-nav-list a.active mat-icon {
      color: #3f51b5;
    }
  `]
})
export class AdminLayoutComponent {
  private readonly authService = inject(AuthService);

  readonly sidenavOpen = signal(true);
  readonly user = this.authService.user;

  private readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Accounts', icon: 'business', route: '/accounts', permissions: ['account:read'] },
    { label: 'Users', icon: 'people', route: '/users', permissions: ['user:read'] },
    { label: 'Roles', icon: 'admin_panel_settings', route: '/roles', permissions: ['role:read'] },
    { label: 'Profiles', icon: 'person', route: '/profiles', permissions: ['profile:read'] },
    { label: 'Content', icon: 'article', route: '/content', permissions: ['content:read'] },
    { label: 'Media', icon: 'perm_media', route: '/media', permissions: ['media:read'] },
    { label: 'Workflows', icon: 'account_tree', route: '/workflows', permissions: ['workflow:read'] },
    { label: 'API Keys', icon: 'vpn_key', route: '/api-keys', permissions: ['apikey:read'] },
    { label: 'Webhooks', icon: 'webhook', route: '/webhooks', permissions: ['webhook:read'] },
    { label: 'Audit Logs', icon: 'history', route: '/audit', permissions: ['audit:read'] },
    { label: 'System', icon: 'settings_applications', route: '/system', permissions: ['system:read'] }
  ];

  readonly filteredNavItems = computed(() => {
    return this.navItems.filter(item => {
      if (!item.permissions) return true;
      return this.authService.hasAnyPermission(item.permissions);
    });
  });

  toggleSidenav(): void {
    this.sidenavOpen.update(open => !open);
  }

  logout(): void {
    this.authService.logout();
  }
}
