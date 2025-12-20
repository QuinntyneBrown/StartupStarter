import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UserListComponent } from './user-list.component';
import { UserService } from '../../../services';
import { UserStatus } from '../../../models';

const mockUsers = [
  { userId: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com', status: UserStatus.Active },
  { userId: '2', firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com', status: UserStatus.Active },
  { userId: '3', firstName: 'Bob', lastName: 'Wilson', email: 'bob@example.com', status: UserStatus.Invited },
  { userId: '4', firstName: 'Alice', lastName: 'Brown', email: 'alice@example.com', status: UserStatus.Inactive },
  { userId: '5', firstName: 'Charlie', lastName: 'Davis', email: 'charlie@example.com', status: UserStatus.Locked }
];

const createMockUserService = (users: typeof mockUsers = mockUsers, loadingDelay = 500) => ({
  getAll: () => of(users).pipe(delay(loadingDelay)),
  activate: () => of(undefined),
  deactivate: () => of(undefined),
  lock: () => of(undefined),
  unlock: () => of(undefined),
  delete: () => of(undefined)
});

const meta: Meta<UserListComponent> = {
  title: 'Pages/Users/UserList',
  component: UserListComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([
          { path: 'users/invite', component: UserListComponent },
          { path: 'users/:id', component: UserListComponent }
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
type Story = StoryObj<UserListComponent>;

export const Default: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: UserService, useValue: createMockUserService() }
      ]
    })
  ]
};

export const Loading: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: UserService, useValue: createMockUserService(mockUsers, 10000) }
      ]
    })
  ]
};

export const EmptyState: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: UserService, useValue: createMockUserService([]) }
      ]
    })
  ]
};

export const WithManyUsers: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: UserService,
          useValue: createMockUserService([
            ...mockUsers,
            { userId: '6', firstName: 'David', lastName: 'Miller', email: 'david@example.com', status: UserStatus.Active },
            { userId: '7', firstName: 'Emily', lastName: 'Garcia', email: 'emily@example.com', status: UserStatus.Active },
            { userId: '8', firstName: 'Frank', lastName: 'Martinez', email: 'frank@example.com', status: UserStatus.Inactive },
            { userId: '9', firstName: 'Grace', lastName: 'Rodriguez', email: 'grace@example.com', status: UserStatus.Active },
            { userId: '10', firstName: 'Henry', lastName: 'Lopez', email: 'henry@example.com', status: UserStatus.Invited }
          ])
        }
      ]
    })
  ]
};

export const AllActive: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: UserService,
          useValue: createMockUserService([
            { userId: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com', status: UserStatus.Active },
            { userId: '2', firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com', status: UserStatus.Active },
            { userId: '3', firstName: 'Bob', lastName: 'Wilson', email: 'bob@example.com', status: UserStatus.Active }
          ])
        }
      ]
    })
  ]
};

export const AllLocked: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: UserService,
          useValue: createMockUserService([
            { userId: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com', status: UserStatus.Locked },
            { userId: '2', firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com', status: UserStatus.Locked },
            { userId: '3', firstName: 'Bob', lastName: 'Wilson', email: 'bob@example.com', status: UserStatus.Locked }
          ])
        }
      ]
    })
  ]
};
