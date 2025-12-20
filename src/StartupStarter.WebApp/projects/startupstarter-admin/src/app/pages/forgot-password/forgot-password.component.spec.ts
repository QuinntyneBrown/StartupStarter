import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { ForgotPasswordComponent } from './forgot-password.component';
import { AuthService } from '../../services';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let authService: any;

  beforeEach(async () => {
    authService = {
      forgotPassword: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ForgotPasswordComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        provideNoopAnimations()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Validation', () => {
    it('should initialize with invalid form', () => {
      expect(component.form.valid).toBeFalsy();
    });

    it('should require email field', () => {
      const emailControl = component.form.get('email');
      expect(emailControl?.valid).toBeFalsy();
      expect(emailControl?.hasError('required')).toBeTruthy();
    });

    it('should validate email format', () => {
      const emailControl = component.form.get('email');

      emailControl?.setValue('invalid-email');
      expect(emailControl?.hasError('email')).toBeTruthy();
      expect(emailControl?.valid).toBeFalsy();
    });

    it('should accept valid email', () => {
      const emailControl = component.form.get('email');

      emailControl?.setValue('test@example.com');
      expect(emailControl?.valid).toBeTruthy();
      expect(emailControl?.hasError('email')).toBeFalsy();
    });

    it('should validate form when email is valid', () => {
      component.form.setValue({ email: 'test@example.com' });
      expect(component.form.valid).toBeTruthy();
    });
  });

  describe('Submit Functionality', () => {
    it('should not submit when form is invalid', () => {
      component.onSubmit();
      expect(authService.forgotPassword).not.toHaveBeenCalled();
    });

    it('should call authService.forgotPassword with email on valid submit', () => {
      authService.forgotPassword.mockReturnValue(of(undefined));

      component.form.setValue({ email: 'test@example.com' });
      component.onSubmit();

      expect(authService.forgotPassword).toHaveBeenCalledWith({
        email: 'test@example.com'
      });
    });

    it('should set loading state during submission', () => {
      authService.forgotPassword.mockReturnValue(of(undefined));

      component.form.setValue({ email: 'test@example.com' });

      expect(component.isLoading()).toBeFalsy();

      component.onSubmit();

      expect(component.isLoading()).toBeFalsy(); // Set to false after completion
    });

    it('should clear error message on submit', () => {
      authService.forgotPassword.mockReturnValue(of(undefined));

      component.errorMessage.set('Previous error');
      component.form.setValue({ email: 'test@example.com' });

      component.onSubmit();

      expect(component.errorMessage()).toBe('');
    });

    it('should set success message on successful submission', () => {
      authService.forgotPassword.mockReturnValue(of(undefined));

      component.form.setValue({ email: 'test@example.com' });
      component.onSubmit();

      expect(component.isLoading()).toBeFalsy();
      expect(component.successMessage()).toBe(
        'If an account exists with this email, you will receive a password reset link shortly.'
      );
    });

    it('should handle error and set error message', () => {
      const errorResponse = {
        error: { message: 'Service unavailable' }
      };
      authService.forgotPassword.mockReturnValue(throwError(() => errorResponse));

      component.form.setValue({ email: 'test@example.com' });
      component.onSubmit();

      expect(component.isLoading()).toBeFalsy();
      expect(component.errorMessage()).toBe('Service unavailable');
      expect(component.successMessage()).toBe('');
    });

    it('should set default error message when error has no message', () => {
      authService.forgotPassword.mockReturnValue(throwError(() => ({})));

      component.form.setValue({ email: 'test@example.com' });
      component.onSubmit();

      expect(component.errorMessage()).toBe('An error occurred. Please try again.');
    });
  });

  describe('Component State', () => {
    it('should initialize with empty error and success messages', () => {
      expect(component.errorMessage()).toBe('');
      expect(component.successMessage()).toBe('');
    });

    it('should initialize with loading set to false', () => {
      expect(component.isLoading()).toBeFalsy();
    });
  });
});
