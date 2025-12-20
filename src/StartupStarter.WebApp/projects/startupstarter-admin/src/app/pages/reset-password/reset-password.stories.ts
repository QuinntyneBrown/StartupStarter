import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ResetPasswordComponent } from './reset-password.component';
import { AuthService } from '../../services';

const createMockAuthService = (behavior: 'success' | 'error') => ({
  resetPassword: () => {
    if (behavior === 'error') {
      return throwError(() => ({ error: { message: 'Token has expired or is invalid.' } })).pipe(delay(1000));
    }
    return of(undefined).pipe(delay(1000));
  }
});

const mockActivatedRoute = {
  snapshot: {
    queryParamMap: {
      get: (key: string) => key === 'token' ? 'valid-reset-token' : null
    }
  }
};

const mockActivatedRouteNoToken = {
  snapshot: {
    queryParamMap: {
      get: () => null
    }
  }
};

const meta: Meta<ResetPasswordComponent> = {
  title: 'Pages/Authentication/ResetPassword',
  component: ResetPasswordComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([{ path: 'login', component: ResetPasswordComponent }])
      ]
    })
  ],
  parameters: {
    layout: 'fullscreen'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<ResetPasswordComponent>;

export const Default: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('success') },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    })
  ]
};

export const WithError: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('error') },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    })
  ]
};

export const MissingToken: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('success') },
        { provide: ActivatedRoute, useValue: mockActivatedRouteNoToken }
      ]
    })
  ]
};

export const SuccessState: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('success') },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    })
  ]
};
