import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { RoleListComponent } from './role-list.component';
import { RoleService } from '../../services';

const mockRoles = [
  { roleId: '1', roleName: 'Administrator', description: 'Full system access', permissions: ['*:*'], isSystemRole: true },
  { roleId: '2', roleName: 'Editor', description: 'Content management access', permissions: ['content:*', 'media:*'], isSystemRole: false },
  { roleId: '3', roleName: 'Viewer', description: 'Read-only access', permissions: ['content:read', 'media:read'], isSystemRole: false },
  { roleId: '4', roleName: 'Moderator', description: 'Content moderation', permissions: ['content:read', 'content:approve'], isSystemRole: false }
];

const createMockRoleService = (roles: typeof mockRoles = mockRoles, loadingDelay = 500) => ({
  getAll: () => of(roles).pipe(delay(loadingDelay)),
  delete: () => of(undefined)
});

const meta: Meta<RoleListComponent> = {
  title: 'Pages/Roles/RoleList',
  component: RoleListComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([
          { path: 'roles/new', component: RoleListComponent },
          { path: 'roles/:id', component: RoleListComponent }
        ])
      ]
    })
  ],
  parameters: {
    layout: 'padded'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<RoleListComponent>;

export const Default: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: RoleService, useValue: createMockRoleService() }
      ]
    })
  ]
};

export const Loading: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: RoleService, useValue: createMockRoleService(mockRoles, 10000) }
      ]
    })
  ]
};

export const EmptyState: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: RoleService, useValue: createMockRoleService([]) }
      ]
    })
  ]
};

export const SystemRolesOnly: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: RoleService,
          useValue: createMockRoleService([
            { roleId: '1', roleName: 'Administrator', description: 'Full system access', permissions: ['*:*'], isSystemRole: true },
            { roleId: '2', roleName: 'System User', description: 'Default user access', permissions: ['user:read'], isSystemRole: true }
          ])
        }
      ]
    })
  ]
};

export const ManyRoles: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: RoleService,
          useValue: createMockRoleService([
            ...mockRoles,
            { roleId: '5', roleName: 'Content Creator', description: 'Can create content', permissions: ['content:create'], isSystemRole: false },
            { roleId: '6', roleName: 'Content Reviewer', description: 'Can review content', permissions: ['content:review'], isSystemRole: false },
            { roleId: '7', roleName: 'Media Manager', description: 'Full media access', permissions: ['media:*'], isSystemRole: false },
            { roleId: '8', roleName: 'User Manager', description: 'User management', permissions: ['user:*'], isSystemRole: false }
          ])
        }
      ]
    })
  ]
};
