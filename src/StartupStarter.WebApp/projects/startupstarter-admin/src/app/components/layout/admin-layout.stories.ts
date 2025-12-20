import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { signal } from '@angular/core';
import { provideRouter } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AdminLayoutComponent } from './admin-layout.component';
import { AuthService } from '../../services';

const mockAuthService = {
  user: signal({
    id: '1',
    firstName: 'John',
    lastName: 'Doe',
    email: 'john.doe@example.com',
    roles: ['Admin'],
    permissions: ['*:*']
  }),
  hasAnyPermission: () => true,
  logout: () => console.log('Logout called')
};

const meta: Meta<AdminLayoutComponent> = {
  title: 'Layout/AdminLayout',
  component: AdminLayoutComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([
          { path: 'dashboard', component: AdminLayoutComponent },
          { path: 'users', component: AdminLayoutComponent },
          { path: 'accounts', component: AdminLayoutComponent },
          { path: 'roles', component: AdminLayoutComponent },
          { path: 'profiles', component: AdminLayoutComponent },
          { path: 'content', component: AdminLayoutComponent },
          { path: 'media', component: AdminLayoutComponent },
          { path: 'workflows', component: AdminLayoutComponent },
          { path: 'api-keys', component: AdminLayoutComponent },
          { path: 'webhooks', component: AdminLayoutComponent },
          { path: 'audit', component: AdminLayoutComponent },
          { path: 'system', component: AdminLayoutComponent },
          { path: 'settings', component: AdminLayoutComponent }
        ])
      ]
    }),
    moduleMetadata({
      imports: [
        MatSidenavModule,
        MatToolbarModule,
        MatIconModule,
        MatButtonModule,
        MatListModule,
        MatMenuModule,
        MatDividerModule
      ],
      providers: [
        { provide: AuthService, useValue: mockAuthService }
      ]
    })
  ],
  parameters: {
    layout: 'fullscreen'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<AdminLayoutComponent>;

export const Default: Story = {
  render: () => ({
    template: `
      <app-admin-layout>
        <div style="padding: 24px;">
          <h2>Page Content Area</h2>
          <p>This is where the page content would appear.</p>
        </div>
      </app-admin-layout>
    `
  })
};

export const WithDashboardContent: Story = {
  render: () => ({
    template: `
      <app-admin-layout>
        <div style="padding: 24px;">
          <h1 style="margin: 0 0 24px 0; font-size: 24px; font-weight: 500;">Dashboard</h1>
          <div style="display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px;">
            <div style="background: #f5f5f5; padding: 24px; border-radius: 8px; border-left: 4px solid #3f51b5;">
              <h3 style="margin: 0; font-size: 28px;">24</h3>
              <p style="margin: 4px 0 0; color: #666;">Total Users</p>
            </div>
            <div style="background: #f5f5f5; padding: 24px; border-radius: 8px; border-left: 4px solid #4caf50;">
              <h3 style="margin: 0; font-size: 28px;">156</h3>
              <p style="margin: 4px 0 0; color: #666;">Active Content</p>
            </div>
            <div style="background: #f5f5f5; padding: 24px; border-radius: 8px; border-left: 4px solid #ff9800;">
              <h3 style="margin: 0; font-size: 28px;">8</h3>
              <p style="margin: 4px 0 0; color: #666;">Pending Approvals</p>
            </div>
            <div style="background: #f5f5f5; padding: 24px; border-radius: 8px; border-left: 4px solid #9c27b0;">
              <h3 style="margin: 0; font-size: 28px;">342</h3>
              <p style="margin: 4px 0 0; color: #666;">Media Files</p>
            </div>
          </div>
        </div>
      </app-admin-layout>
    `
  })
};

export const WithTableContent: Story = {
  render: () => ({
    template: `
      <app-admin-layout>
        <div style="padding: 24px;">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px;">
            <div>
              <h1 style="margin: 0; font-size: 24px; font-weight: 500;">Users</h1>
              <p style="margin: 4px 0 0; color: #666;">Manage user accounts</p>
            </div>
            <button style="background: #3f51b5; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer;">
              Add User
            </button>
          </div>
          <table style="width: 100%; border-collapse: collapse; background: white; box-shadow: 0 1px 3px rgba(0,0,0,0.12);">
            <thead>
              <tr style="background: #fafafa;">
                <th style="text-align: left; padding: 16px; border-bottom: 1px solid #e0e0e0;">Name</th>
                <th style="text-align: left; padding: 16px; border-bottom: 1px solid #e0e0e0;">Email</th>
                <th style="text-align: left; padding: 16px; border-bottom: 1px solid #e0e0e0;">Role</th>
                <th style="text-align: left; padding: 16px; border-bottom: 1px solid #e0e0e0;">Status</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">John Doe</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">john@example.com</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">Admin</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">
                  <span style="background: #e8f5e9; color: #2e7d32; padding: 4px 12px; border-radius: 16px; font-size: 12px;">Active</span>
                </td>
              </tr>
              <tr>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">Jane Smith</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">jane@example.com</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">Editor</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">
                  <span style="background: #e8f5e9; color: #2e7d32; padding: 4px 12px; border-radius: 16px; font-size: 12px;">Active</span>
                </td>
              </tr>
              <tr>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">Bob Wilson</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">bob@example.com</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">Viewer</td>
                <td style="padding: 16px; border-bottom: 1px solid #e0e0e0;">
                  <span style="background: #fff3e0; color: #ef6c00; padding: 4px 12px; border-radius: 16px; font-size: 12px;">Pending</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </app-admin-layout>
    `
  })
};
