import { Component, inject, signal, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { AuthService } from '../../services';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  permission?: string;
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
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatMenuModule,
    MatDividerModule,
    MatTooltipModule
  ],
  template: `
    <mat-sidenav-container class="admin-layout">
      <mat-sidenav
        [mode]="isMobile() ? 'over' : 'side'"
        [opened]="!isMobile() || sidenavOpen()"
        (closed)="sidenavOpen.set(false)"
        class="admin-layout__sidenav"
      >
        <div class="admin-layout__sidenav-header">
          <div class="admin-layout__logo">
            <mat-icon>rocket_launch</mat-icon>
            <span class="text-title-medium">StartupStarter</span>
          </div>
        </div>

        <mat-nav-list class="admin-layout__nav">
          @for (item of filteredNavItems(); track item.route) {
            <a
              mat-list-item
              [routerLink]="item.route"
              routerLinkActive="admin-layout__nav-item--active"
              class="admin-layout__nav-item"
              (click)="isMobile() && sidenavOpen.set(false)"
            >
              <mat-icon matListItemIcon>{{ item.icon }}</mat-icon>
              <span matListItemTitle>{{ item.label }}</span>
            </a>
          }
        </mat-nav-list>

        <div class="admin-layout__sidenav-footer">
          <mat-divider></mat-divider>
          <a mat-list-item routerLink="/settings" class="admin-layout__nav-item">
            <mat-icon matListItemIcon>settings</mat-icon>
            <span matListItemTitle>Settings</span>
          </a>
        </div>
      </mat-sidenav>

      <mat-sidenav-content class="admin-layout__content">
        <mat-toolbar color="primary" class="admin-layout__toolbar">
          @if (isMobile()) {
            <button mat-icon-button (click)="sidenavOpen.set(true)">
              <mat-icon>menu</mat-icon>
            </button>
          }

          <span class="admin-layout__toolbar-title">Admin Dashboard</span>

          <span class="admin-layout__spacer"></span>

          <button mat-icon-button matTooltip="Notifications">
            <mat-icon>notifications</mat-icon>
          </button>

          <button mat-icon-button [matMenuTriggerFor]="userMenu">
            <mat-icon>account_circle</mat-icon>
          </button>

          <mat-menu #userMenu="matMenu">
            <div class="admin-layout__user-info">
              <span class="text-title-small">{{ userFullName() }}</span>
              <span class="text-body-small">{{ userEmail() }}</span>
            </div>
            <mat-divider></mat-divider>
            <button mat-menu-item routerLink="/profile">
              <mat-icon>person</mat-icon>
              <span>My Profile</span>
            </button>
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
      height: 100%;

      &__sidenav {
        width: var(--sidebar-width);
        background: var(--mat-sys-surface-container-low);
      }

      &__sidenav-header {
        padding: var(--spacing-md);
        border-bottom: 1px solid var(--mat-sys-outline-variant);
      }

      &__logo {
        display: flex;
        align-items: center;
        gap: var(--spacing-sm);
        color: var(--mat-sys-primary);
      }

      &__nav {
        padding: var(--spacing-sm);
      }

      &__nav-item {
        border-radius: var(--radius-md);
        margin-bottom: var(--spacing-xs);

        &--active {
          background: var(--mat-sys-primary-container) !important;
          color: var(--mat-sys-on-primary-container) !important;
        }
      }

      &__sidenav-footer {
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        padding: var(--spacing-sm);
      }

      &__content {
        display: flex;
        flex-direction: column;
        height: 100%;
      }

      &__toolbar {
        position: sticky;
        top: 0;
        z-index: 100;
      }

      &__toolbar-title {
        margin-left: var(--spacing-md);
      }

      &__spacer {
        flex: 1;
      }

      &__user-info {
        padding: var(--spacing-md);
        display: flex;
        flex-direction: column;
      }

      &__main {
        flex: 1;
        padding: var(--spacing-lg);
        overflow: auto;
        background: var(--mat-sys-surface);
      }
    }

    @media (max-width: 768px) {
      .admin-layout {
        &__main {
          padding: var(--spacing-md);
        }
      }
    }
  `]
})
export class AdminLayout {
  private readonly authService = inject(AuthService);
  private readonly breakpointObserver = inject(BreakpointObserver);

  readonly sidenavOpen = signal(false);

  private readonly isMobileBreakpoint = toSignal(
    this.breakpointObserver.observe([Breakpoints.Handset, Breakpoints.TabletPortrait])
      .pipe(map(result => result.matches)),
    { initialValue: false }
  );

  readonly isMobile = this.isMobileBreakpoint;

  readonly userFullName = computed(() => this.authService.userFullName());
  readonly userEmail = computed(() => this.authService.user()?.email || '');

  private readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Accounts', icon: 'business', route: '/accounts', permission: 'accounts.view' },
    { label: 'Users', icon: 'people', route: '/users', permission: 'users.view' },
    { label: 'Roles', icon: 'admin_panel_settings', route: '/roles', permission: 'roles.view' },
    { label: 'Profiles', icon: 'account_box', route: '/profiles', permission: 'profiles.view' },
    { label: 'Content', icon: 'article', route: '/content', permission: 'content.view' },
    { label: 'Media', icon: 'perm_media', route: '/media', permission: 'media.view' },
    { label: 'Workflows', icon: 'account_tree', route: '/workflows', permission: 'workflows.view' },
    { label: 'API Keys', icon: 'key', route: '/api-keys', permission: 'apikeys.view' },
    { label: 'Webhooks', icon: 'webhook', route: '/webhooks', permission: 'webhooks.view' },
    { label: 'Audit Logs', icon: 'history', route: '/audit', permission: 'audit.view' },
    { label: 'System', icon: 'settings_applications', route: '/system', permission: 'system.view' }
  ];

  readonly filteredNavItems = computed(() => {
    return this.navItems.filter(item => {
      if (!item.permission) return true;
      return this.authService.hasPermission(item.permission);
    });
  });

  logout(): void {
    this.authService.logout().subscribe();
  }
}
