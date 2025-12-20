import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ForgotPasswordComponent } from './forgot-password.component';
import { AuthService } from '../../services';

const createMockAuthService = (behavior: 'success' | 'error') => ({
  forgotPassword: () => {
    if (behavior === 'error') {
      return throwError(() => ({ error: { message: 'An error occurred. Please try again.' } })).pipe(delay(1000));
    }
    return of(undefined).pipe(delay(1000));
  }
});

const meta: Meta<ForgotPasswordComponent> = {
  title: 'Pages/Authentication/ForgotPassword',
  component: ForgotPasswordComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([{ path: 'login', component: ForgotPasswordComponent }])
      ]
    })
  ],
  parameters: {
    layout: 'fullscreen'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<ForgotPasswordComponent>;

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
  ]
};

export const SuccessState: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: AuthService, useValue: createMockAuthService('success') }
      ]
    })
  ]
};
