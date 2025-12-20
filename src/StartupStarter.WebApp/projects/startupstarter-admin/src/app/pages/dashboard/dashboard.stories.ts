import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { signal } from '@angular/core';
import { provideRouter } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { AuthService } from '../../services';

const mockAuthService = {
  user: signal({
    id: '1',
    firstName: 'John',
    lastName: 'Doe',
    email: 'john.doe@example.com',
    roles: ['Admin'],
    permissions: ['*:*']
  })
};

const meta: Meta<DashboardComponent> = {
  title: 'Pages/Dashboard',
  component: DashboardComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([
          { path: 'users', component: DashboardComponent },
          { path: 'content', component: DashboardComponent },
          { path: 'workflows', component: DashboardComponent },
          { path: 'media', component: DashboardComponent },
          { path: 'audit', component: DashboardComponent }
        ])
      ]
    }),
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: mockAuthService }
      ]
    })
  ],
  parameters: {
    layout: 'padded'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<DashboardComponent>;

export const Default: Story = {};

export const Loading: Story = {
  render: () => ({
    template: `
      <app-page-header
        title="Dashboard"
        subtitle="Welcome back, John"
        icon="dashboard">
      </app-page-header>
      <app-loading-spinner message="Loading dashboard..."></app-loading-spinner>
    `
  })
};

export const WithData: Story = {};
