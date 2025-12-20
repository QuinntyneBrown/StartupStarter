import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="login">
      <mat-card class="login__card">
        <mat-card-header>
          <mat-card-title>Welcome Back</mat-card-title>
          <mat-card-subtitle>Sign in to your account</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          @if (errorMessage()) {
            <div class="login__error-message">{{ errorMessage() }}</div>
          }

          @if (!showMfa()) {
            <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
              <mat-form-field appearance="outline" class="login__field">
                <mat-label>Email</mat-label>
                <input matInput formControlName="email" placeholder="you@example.com">
                <mat-icon matSuffix>email</mat-icon>
                @if (loginForm.get('email')?.hasError('required')) {
                  <mat-error>Email is required</mat-error>
                }
                @if (loginForm.get('email')?.hasError('email')) {
                  <mat-error>Please enter a valid email</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="login__field">
                <mat-label>Password</mat-label>
                <input matInput [type]="hidePassword() ? 'password' : 'text'" formControlName="password">
                <button mat-icon-button matSuffix type="button" (click)="togglePassword()">
                  <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
                </button>
                @if (loginForm.get('password')?.hasError('required')) {
                  <mat-error>Password is required</mat-error>
                }
              </mat-form-field>

              <button mat-flat-button color="primary" type="submit" class="login__submit-btn"
                      [disabled]="isLoading() || loginForm.invalid">
                @if (isLoading()) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Sign In
                }
              </button>
            </form>
          } @else {
            <form [formGroup]="mfaForm" (ngSubmit)="onVerifyMfa()">
              <p class="login__mfa-message">Enter the verification code from your authenticator app</p>

              <mat-form-field appearance="outline" class="login__field">
                <mat-label>Verification Code</mat-label>
                <input matInput formControlName="code" maxlength="6" placeholder="000000">
                @if (mfaForm.get('code')?.hasError('required')) {
                  <mat-error>Code is required</mat-error>
                }
              </mat-form-field>

              <button mat-flat-button color="primary" type="submit" class="login__submit-btn"
                      [disabled]="isLoading() || mfaForm.invalid">
                @if (isLoading()) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Verify
                }
              </button>

              <button mat-button type="button" class="login__back-btn" (click)="backToLogin()">
                Back to Login
              </button>
            </form>
          }
        </mat-card-content>

        <mat-card-actions align="end">
          <a mat-button routerLink="/forgot-password">Forgot Password?</a>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .login {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 24px;
    }

    .login__card {
      width: 100%;
      max-width: 400px;
      padding: 24px;
    }

    mat-card-header {
      margin-bottom: 24px;
    }

    mat-card-title {
      font-size: 24px !important;
      margin-bottom: 8px;
    }

    .login__field {
      width: 100%;
    }

    .login__submit-btn {
      width: 100%;
      margin-top: 16px;
      height: 48px;
    }

    .login__back-btn {
      width: 100%;
    }

    .login__error-message {
      background-color: #ffebee;
      color: #c62828;
      padding: 12px;
      border-radius: 4px;
      margin-bottom: 16px;
      font-size: 14px;
    }

    .login__mfa-message {
      color: rgba(0, 0, 0, 0.6);
      margin-bottom: 16px;
      text-align: center;
    }

    mat-spinner {
      display: inline-block;
    }
  `]
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly hidePassword = signal(true);
  readonly showMfa = signal(false);
  private sessionId = '';

  loginForm = this.fb.group({
    email: ['', [Validators.required]],
    password: ['', Validators.required]
  });

  mfaForm = this.fb.group({
    code: ['', Validators.required]
  });

  togglePassword(): void {
    this.hidePassword.update(hide => !hide);
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    const { email, password } = this.loginForm.value;
    this.authService.login({ email: email!, password: password! }).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        if (response.requiresMfa) {
          this.showMfa.set(true);
          this.sessionId = response.accessToken;
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error?.error?.message || 'Invalid email or password');
      }
    });
  }

  onVerifyMfa(): void {
    if (this.mfaForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    const { code } = this.mfaForm.value;
    this.authService.verifyMfa({ code: code!, sessionId: this.sessionId }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error?.error?.message || 'Invalid verification code');
      }
    });
  }

  backToLogin(): void {
    this.showMfa.set(false);
    this.mfaForm.reset();
    this.errorMessage.set('');
  }
}
