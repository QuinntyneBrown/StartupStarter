import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services';
import { LoginResponse } from '../../models';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: any;
  let router: any;

  const mockLoginResponse: LoginResponse = {
    accessToken: 'test-token',
    refreshToken: 'test-refresh-token',
    requiresMfa: false,
    user: {
      id: '1',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      roles: ['user'],
      permissions: []
    }
  };

  const mockMfaResponse: LoginResponse = {
    accessToken: 'session-id',
    refreshToken: '',
    requiresMfa: true,
    user: {
      id: '1',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      roles: ['user'],
      permissions: []
    }
  };

  beforeEach(async () => {
    authService = {
      login: vi.fn(),
      verifyMfa: vi.fn()
    };

    router = {
      navigate: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
        provideNoopAnimations()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Validation', () => {
    it('should initialize with invalid login form', () => {
      expect(component.loginForm.valid).toBeFalsy();
    });

    it('should require email field', () => {
      const emailControl = component.loginForm.get('email');
      expect(emailControl?.valid).toBeFalsy();
      expect(emailControl?.hasError('required')).toBeTruthy();
    });

    it('should require password field', () => {
      const passwordControl = component.loginForm.get('password');
      expect(passwordControl?.valid).toBeFalsy();
      expect(passwordControl?.hasError('required')).toBeTruthy();
    });

    it('should validate login form when all fields are filled', () => {
      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });
      expect(component.loginForm.valid).toBeTruthy();
    });

    it('should initialize with invalid MFA form', () => {
      expect(component.mfaForm.valid).toBeFalsy();
    });

    it('should require MFA code field', () => {
      const codeControl = component.mfaForm.get('code');
      expect(codeControl?.valid).toBeFalsy();
      expect(codeControl?.hasError('required')).toBeTruthy();
    });

    it('should validate MFA form when code is filled', () => {
      component.mfaForm.setValue({ code: '123456' });
      expect(component.mfaForm.valid).toBeTruthy();
    });
  });

  describe('Login Submit', () => {
    it('should not submit when login form is invalid', () => {
      component.onSubmit();
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should call authService.login with form values on valid submit', () => {
      authService.login.mockReturnValue(of(mockLoginResponse));

      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });

      component.onSubmit();

      expect(authService.login).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password123'
      });
    });

    it('should navigate to dashboard on successful login without MFA', () => {
      authService.login.mockReturnValue(of(mockLoginResponse));

      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });

      component.onSubmit();

      expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
    });

    it('should set loading state during login', () => {
      authService.login.mockReturnValue(of(mockLoginResponse));

      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });

      expect(component.isLoading()).toBeFalsy();

      component.onSubmit();

      expect(component.isLoading()).toBeFalsy(); // Set to false after completion
    });

    it('should clear error message on submit', () => {
      authService.login.mockReturnValue(of(mockLoginResponse));

      component.errorMessage.set('Previous error');
      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });

      component.onSubmit();

      expect(component.errorMessage()).toBe('');
    });

    it('should handle login error and set error message', () => {
      const errorResponse = {
        error: { message: 'Invalid credentials' }
      };
      authService.login.mockReturnValue(throwError(() => errorResponse));

      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'wrongpassword'
      });

      component.onSubmit();

      expect(component.isLoading()).toBeFalsy();
      expect(component.errorMessage()).toBe('Invalid credentials');
      expect(router.navigate).not.toHaveBeenCalled();
    });

    it('should set default error message when error has no message', () => {
      authService.login.mockReturnValue(throwError(() => ({})));

      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });

      component.onSubmit();

      expect(component.errorMessage()).toBe('Invalid email or password');
    });
  });

  describe('MFA Flow', () => {
    it('should show MFA form when login requires MFA', () => {
      authService.login.mockReturnValue(of(mockMfaResponse));

      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });

      expect(component.showMfa()).toBeFalsy();

      component.onSubmit();

      expect(component.showMfa()).toBeTruthy();
      expect(router.navigate).not.toHaveBeenCalled();
    });

    it('should not submit MFA when form is invalid', () => {
      component.onVerifyMfa();
      expect(authService.verifyMfa).not.toHaveBeenCalled();
    });

    it('should call authService.verifyMfa with code and sessionId', () => {
      authService.login.mockReturnValue(of(mockMfaResponse));
      authService.verifyMfa.mockReturnValue(of(mockLoginResponse));

      // First login to get session ID
      component.loginForm.setValue({
        email: 'test@example.com',
        password: 'password123'
      });
      component.onSubmit();

      // Then verify MFA
      component.mfaForm.setValue({ code: '123456' });
      component.onVerifyMfa();

      expect(authService.verifyMfa).toHaveBeenCalledWith({
        code: '123456',
        sessionId: 'session-id'
      });
    });

    it('should navigate to dashboard after successful MFA verification', () => {
      authService.verifyMfa.mockReturnValue(of(mockLoginResponse));

      component.showMfa.set(true);
      component.mfaForm.setValue({ code: '123456' });

      component.onVerifyMfa();

      expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
    });

    it('should handle MFA verification error', () => {
      const errorResponse = {
        error: { message: 'Invalid code' }
      };
      authService.verifyMfa.mockReturnValue(throwError(() => errorResponse));

      component.showMfa.set(true);
      component.mfaForm.setValue({ code: 'wrong' });

      component.onVerifyMfa();

      expect(component.isLoading()).toBeFalsy();
      expect(component.errorMessage()).toBe('Invalid code');
    });

    it('should set default error message for MFA verification error', () => {
      authService.verifyMfa.mockReturnValue(throwError(() => ({})));

      component.showMfa.set(true);
      component.mfaForm.setValue({ code: '123456' });

      component.onVerifyMfa();

      expect(component.errorMessage()).toBe('Invalid verification code');
    });

    it('should go back to login form from MFA', () => {
      component.showMfa.set(true);
      component.mfaForm.setValue({ code: '123456' });
      component.errorMessage.set('Some error');

      component.backToLogin();

      expect(component.showMfa()).toBeFalsy();
      expect(component.mfaForm.value.code).toBeNull();
      expect(component.errorMessage()).toBe('');
    });
  });

  describe('Password Toggle', () => {
    it('should toggle password visibility', () => {
      expect(component.hidePassword()).toBeTruthy();

      component.togglePassword();
      expect(component.hidePassword()).toBeFalsy();

      component.togglePassword();
      expect(component.hidePassword()).toBeTruthy();
    });
  });
});
