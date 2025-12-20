import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services';

const createMockAuthService = (behavior: 'success' | 'error' | 'mfa') => ({
  login: () => {
    if (behavior === 'error') {
      return throwError(() => ({ error: { message: 'Invalid email or password' } })).pipe(delay(1000));
    }
    if (behavior === 'mfa') {
      return of({ requiresMfa: true, accessToken: 'mfa-session' }).pipe(delay(1000));
    }
    return of({
      requiresMfa: false,
      accessToken: 'token',
      refreshToken: 'refresh',
      user: { id: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com' }
    }).pipe(delay(1000));
  },
  verifyMfa: () => of({
    accessToken: 'token',
    refreshToken: 'refresh',
    user: { id: '1', firstName: 'John', lastName: 'Doe', email: 'john@example.com' }
  }).pipe(delay(1000))
});

const meta: Meta<LoginComponent> = {
  title: 'Pages/Authentication/Login',
  component: LoginComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([{ path: 'dashboard', component: LoginComponent }])
      ]
    })
  ],
  parameters: {
    layout: 'fullscreen'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<LoginComponent>;

export const Default: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('success') }
      ]
    })
  ]
};

export const WithError: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('error') }
      ]
    })
  ],
  play: async ({ canvasElement }) => {
    // This would show the error state after form submission
  }
};

export const WithMfa: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('mfa') }
      ]
    })
  ]
};

export const FilledForm: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('success') }
      ]
    })
  ],
  render: () => ({
    template: `
      <app-login></app-login>
    `,
    props: {}
  })
};
