import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UserFormComponent } from './user-form.component';
import { UserService, RoleService } from '../../services';
import { UserStatus } from '../../models';

const mockRoles = [
  { roleId: '1', roleName: 'Admin', description: 'Full system access' },
  { roleId: '2', roleName: 'Editor', description: 'Content management' },
  { roleId: '3', roleName: 'Viewer', description: 'Read-only access' },
  { roleId: '4', roleName: 'Moderator', description: 'Content moderation' }
];

const mockUser = {
  userId: '1',
  firstName: 'John',
  lastName: 'Doe',
  email: 'john@example.com',
  status: UserStatus.Active
};

const createMockUserService = () => ({
  getById: () => of(mockUser).pipe(delay(500)),
  update: () => of(undefined).pipe(delay(1000)),
  invite: () => of(undefined).pipe(delay(1000))
});

const createMockRoleService = () => ({
  getAll: () => of(mockRoles).pipe(delay(300))
});

const mockActivatedRouteInvite = {
  snapshot: {
    paramMap: {
      get: (key: string) => key === 'id' ? 'invite' : null
    }
  }
};

const mockActivatedRouteEdit = {
  snapshot: {
    paramMap: {
      get: (key: string) => key === 'id' ? '1' : null
    }
  }
};

const meta: Meta<UserFormComponent> = {
  title: 'Pages/Users/UserForm',
  component: UserFormComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([{ path: 'users', component: UserFormComponent }])
      ]
    })
  ],
  parameters: {
    layout: 'padded'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<UserFormComponent>;

export const InviteMode: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: UserService, useValue: createMockUserService() },
        { provide: RoleService, useValue: createMockRoleService() },
        { provide: ActivatedRoute, useValue: mockActivatedRouteInvite }
      ]
    })
  ]
};

export const EditMode: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: UserService, useValue: createMockUserService() },
        { provide: RoleService, useValue: createMockRoleService() },
        { provide: ActivatedRoute, useValue: mockActivatedRouteEdit }
      ]
    })
  ]
};
