import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { signal } from '@angular/core';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { SettingsComponent } from './settings.component';
import { AuthService } from '../../services';

const mockSessions = [
  { sessionId: '1', userAgent: 'Chrome on Windows', ipAddress: '192.168.1.100', createdAt: new Date() },
  { sessionId: '2', userAgent: 'Safari on MacOS', ipAddress: '192.168.1.101', createdAt: new Date(Date.now() - 86400000) },
  { sessionId: '3', userAgent: 'Mobile Safari on iPhone', ipAddress: '10.0.0.15', createdAt: new Date(Date.now() - 172800000) }
];

const createMockAuthService = (sessions = mockSessions) => ({
  user: signal({
    id: '1',
    firstName: 'John',
    lastName: 'Doe',
    email: 'john.doe@example.com',
    roles: ['Admin'],
    permissions: ['*:*']
  }),
  getSessions: () => of(sessions).pipe(delay(500)),
  changePassword: () => of(undefined).pipe(delay(1000)),
  enableMfa: () => of({ qrCode: 'data:image/png;base64,...', secret: 'ABCD1234' }).pipe(delay(500)),
  disableMfa: () => of(undefined).pipe(delay(500)),
  revokeSession: () => of(undefined).pipe(delay(500))
});

const meta: Meta<SettingsComponent> = {
  title: 'Pages/Settings',
  component: SettingsComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([])
      ]
    })
  ],
  parameters: {
    layout: 'padded'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<SettingsComponent>;

export const Default: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService() }
      ]
    })
  ]
};

export const NoSessions: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService([]) }
      ]
    })
  ]
};

export const ManySessions: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: AuthService,
          useValue: createMockAuthService([
            ...mockSessions,
            { sessionId: '4', userAgent: 'Firefox on Linux', ipAddress: '172.16.0.50', createdAt: new Date(Date.now() - 259200000) },
            { sessionId: '5', userAgent: 'Edge on Windows', ipAddress: '192.168.2.25', createdAt: new Date(Date.now() - 345600000) }
          ])
        }
      ]
    })
  ]
};
