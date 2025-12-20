import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { ResetPasswordComponent } from './reset-password.component';
import { AuthService } from '../../services';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let authService: any;
  let router: any;
  let activatedRoute: any;

  beforeEach(async () => {
    authService = {
      resetPassword: vi.fn()
    };

    router = {
      navigate: vi.fn()
    };

    activatedRoute = {
      snapshot: {
        queryParamMap: {
          get: vi.fn().mockReturnValue('test-token')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [ResetPasswordComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
        { provide: ActivatedRoute, useValue: activatedRoute },
        provideNoopAnimations()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should retrieve token from query params on init', () => {
      fixture.detectChanges();
      expect(activatedRoute.snapshot.queryParamMap.get).toHaveBeenCalledWith('token');
    });

    it('should set error message when token is missing', () => {
      activatedRoute.snapshot.queryParamMap.get.mockReturnValue(null);
      fixture.detectChanges();

      expect(component.errorMessage()).toBe('Invalid or missing reset token');
    });

    it('should set error message when token is empty', () => {
      activatedRoute.snapshot.queryParamMap.get.mockReturnValue('');
      fixture.detectChanges();

      expect(component.errorMessage()).toBe('Invalid or missing reset token');
    });

    it('should not set error message when token is valid', () => {
      activatedRoute.snapshot.queryParamMap.get.mockReturnValue('valid-token');
      fixture.detectChanges();

      expect(component.errorMessage()).toBe('');
    });
  });

  describe('Form Validation', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should initialize with invalid form', () => {
      expect(component.form.valid).toBeFalsy();
    });

    it('should require newPassword field', () => {
      const passwordControl = component.form.get('newPassword');
      expect(passwordControl?.valid).toBeFalsy();
      expect(passwordControl?.hasError('required')).toBeTruthy();
    });

    it('should require confirmPassword field', () => {
      const confirmControl = component.form.get('confirmPassword');
      expect(confirmControl?.valid).toBeFalsy();
      expect(confirmControl?.hasError('required')).toBeTruthy();
    });

    it('should require minimum password length of 8 characters', () => {
      const passwordControl = component.form.get('newPassword');

      passwordControl?.setValue('Short1!');
      expect(passwordControl?.hasError('minlength')).toBeTruthy();
      expect(passwordControl?.valid).toBeFalsy();
    });

    it('should validate password pattern - requires uppercase', () => {
      const passwordControl = component.form.get('newPassword');

      passwordControl?.setValue('password123!');
      expect(passwordControl?.hasError('pattern')).toBeTruthy();
    });

    it('should validate password pattern - requires lowercase', () => {
      const passwordControl = component.form.get('newPassword');

      passwordControl?.setValue('PASSWORD123!');
      expect(passwordControl?.hasError('pattern')).toBeTruthy();
    });

    it('should validate password pattern - requires digit', () => {
      const passwordControl = component.form.get('newPassword');

      passwordControl?.setValue('Password!');
      expect(passwordControl?.hasError('pattern')).toBeTruthy();
    });

    it('should validate password pattern - requires special character', () => {
      const passwordControl = component.form.get('newPassword');

      passwordControl?.setValue('Password123');
      expect(passwordControl?.hasError('pattern')).toBeTruthy();
    });

    it('should accept valid password meeting all criteria', () => {
      const passwordControl = component.form.get('newPassword');

      passwordControl?.setValue('Password123!');
      expect(passwordControl?.valid).toBeTruthy();
    });

    it('should validate passwords match', () => {
      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      expect(component.form.hasError('passwordMismatch')).toBeFalsy();
      expect(component.form.valid).toBeTruthy();
    });

    it('should invalidate form when passwords do not match', () => {
      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'DifferentPass123!'
      });

      expect(component.form.hasError('passwordMismatch')).toBeTruthy();
      expect(component.form.valid).toBeFalsy();
    });

    it('should revalidate password match when newPassword changes', () => {
      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });
      expect(component.form.valid).toBeTruthy();

      component.form.patchValue({
        newPassword: 'NewPassword123!'
      });
      expect(component.form.hasError('passwordMismatch')).toBeTruthy();
    });
  });

  describe('Submit Functionality', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should not submit when form is invalid', () => {
      component.onSubmit();
      expect(authService.resetPassword).not.toHaveBeenCalled();
    });

    it('should not submit when token is missing', () => {
      activatedRoute.snapshot.queryParamMap.get.mockReturnValue('');
      component.ngOnInit();

      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      component.onSubmit();
      expect(authService.resetPassword).not.toHaveBeenCalled();
    });

    it('should call authService.resetPassword with correct data on valid submit', () => {
      authService.resetPassword.mockReturnValue(of(undefined));

      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      component.onSubmit();

      expect(authService.resetPassword).toHaveBeenCalledWith({
        token: 'test-token',
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });
    });

    it('should set loading state during submission', () => {
      authService.resetPassword.mockReturnValue(of(undefined));

      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      expect(component.isLoading()).toBeFalsy();

      component.onSubmit();

      expect(component.isLoading()).toBeFalsy(); // Set to false after completion
    });

    it('should clear error message on submit', () => {
      authService.resetPassword.mockReturnValue(of(undefined));

      component.errorMessage.set('Previous error');
      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      component.onSubmit();

      expect(component.errorMessage()).toBe('');
    });

    it('should set success message on successful password reset', () => {
      authService.resetPassword.mockReturnValue(of(undefined));

      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      component.onSubmit();

      expect(component.isLoading()).toBeFalsy();
      expect(component.successMessage()).toBe('Your password has been reset successfully.');
    });

    it('should handle error and set error message', () => {
      const errorResponse = {
        error: { message: 'Invalid or expired token' }
      };
      authService.resetPassword.mockReturnValue(throwError(() => errorResponse));

      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      component.onSubmit();

      expect(component.isLoading()).toBeFalsy();
      expect(component.errorMessage()).toBe('Invalid or expired token');
      expect(component.successMessage()).toBe('');
    });

    it('should set default error message when error has no message', () => {
      authService.resetPassword.mockReturnValue(throwError(() => ({})));

      component.form.setValue({
        newPassword: 'Password123!',
        confirmPassword: 'Password123!'
      });

      component.onSubmit();

      expect(component.errorMessage()).toBe('An error occurred. Please try again.');
    });
  });

  describe('Password Toggle', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should toggle newPassword visibility', () => {
      expect(component.hidePassword()).toBeTruthy();

      component.togglePassword();
      expect(component.hidePassword()).toBeFalsy();

      component.togglePassword();
      expect(component.hidePassword()).toBeTruthy();
    });

    it('should toggle confirmPassword visibility', () => {
      expect(component.hideConfirmPassword()).toBeTruthy();

      component.toggleConfirmPassword();
      expect(component.hideConfirmPassword()).toBeFalsy();

      component.toggleConfirmPassword();
      expect(component.hideConfirmPassword()).toBeTruthy();
    });

    it('should toggle passwords independently', () => {
      component.togglePassword();
      expect(component.hidePassword()).toBeFalsy();
      expect(component.hideConfirmPassword()).toBeTruthy();

      component.toggleConfirmPassword();
      expect(component.hidePassword()).toBeFalsy();
      expect(component.hideConfirmPassword()).toBeFalsy();
    });
  });

  describe('Component State', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should initialize with empty error and success messages when token is present', () => {
      expect(component.errorMessage()).toBe('');
      expect(component.successMessage()).toBe('');
    });

    it('should initialize with loading set to false', () => {
      expect(component.isLoading()).toBeFalsy();
    });

    it('should initialize with password fields hidden', () => {
      expect(component.hidePassword()).toBeTruthy();
      expect(component.hideConfirmPassword()).toBeTruthy();
    });
  });
});
