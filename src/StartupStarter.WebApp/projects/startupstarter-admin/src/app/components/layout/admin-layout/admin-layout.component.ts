import { Component, inject, signal, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../../services';

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
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
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
