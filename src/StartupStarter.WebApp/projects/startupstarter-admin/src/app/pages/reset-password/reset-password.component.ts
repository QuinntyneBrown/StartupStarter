import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services';

@Component({
  selector: 'app-reset-password',
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
    <div class="reset-password">
      <mat-card class="reset-password__card">
        <mat-card-header>
          <mat-card-title>Reset Password</mat-card-title>
          <mat-card-subtitle>Enter your new password</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          @if (successMessage()) {
            <div class="reset-password__success-message">
              <mat-icon>check_circle</mat-icon>
              {{ successMessage() }}
            </div>
            <a mat-flat-button color="primary" routerLink="/login" class="reset-password__login-btn">
              Go to Login
            </a>
          } @else {
            @if (errorMessage()) {
              <div class="reset-password__error-message">{{ errorMessage() }}</div>
            }

            <form [formGroup]="form" (ngSubmit)="onSubmit()">
              <mat-form-field appearance="outline" class="reset-password__field">
                <mat-label>New Password</mat-label>
                <input matInput [type]="hidePassword() ? 'password' : 'text'" formControlName="newPassword">
                <button mat-icon-button matSuffix type="button" (click)="togglePassword()">
                  <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
                </button>
                @if (form.get('newPassword')?.hasError('required')) {
                  <mat-error>Password is required</mat-error>
                }
                @if (form.get('newPassword')?.hasError('minlength')) {
                  <mat-error>Password must be at least 8 characters</mat-error>
                }
                @if (form.get('newPassword')?.hasError('pattern')) {
                  <mat-error>Password must contain uppercase, lowercase, number, and special character</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="reset-password__field">
                <mat-label>Confirm Password</mat-label>
                <input matInput [type]="hideConfirmPassword() ? 'password' : 'text'" formControlName="confirmPassword">
                <button mat-icon-button matSuffix type="button" (click)="toggleConfirmPassword()">
                  <mat-icon>{{ hideConfirmPassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
                </button>
                @if (form.get('confirmPassword')?.hasError('required')) {
                  <mat-error>Please confirm your password</mat-error>
                }
                @if (form.hasError('passwordMismatch')) {
                  <mat-error>Passwords do not match</mat-error>
                }
              </mat-form-field>

              <button mat-flat-button color="primary" type="submit" class="reset-password__submit-btn"
                      [disabled]="isLoading() || form.invalid">
                @if (isLoading()) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Reset Password
                }
              </button>
            </form>
          }
        </mat-card-content>

        @if (!successMessage()) {
          <mat-card-actions class="reset-password__actions">
            <a mat-button routerLink="/login">Back to Login</a>
          </mat-card-actions>
        }
      </mat-card>
    </div>
  `,
  styles: [`
    .reset-password {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 24px;
    }

    .reset-password__card {
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

    .reset-password__field {
      width: 100%;
    }

    .reset-password__login-btn {
      width: 100%;
    }

    .reset-password__submit-btn {
      width: 100%;
      margin-top: 16px;
      height: 48px;
    }

    .reset-password__error-message {
      background-color: #ffebee;
      color: #c62828;
      padding: 12px;
      border-radius: 4px;
      margin-bottom: 16px;
      font-size: 14px;
    }

    .reset-password__success-message {
      background-color: #e8f5e9;
      color: #2e7d32;
      padding: 16px;
      border-radius: 4px;
      margin-bottom: 16px;
      display: flex;
      align-items: center;
      gap: 8px;
    }

    mat-spinner {
      display: inline-block;
    }

    .reset-password__actions {
      display: flex;
      justify-content: center;
    }
  `]
})
export class ResetPasswordComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly successMessage = signal('');
  readonly hidePassword = signal(true);
  readonly hideConfirmPassword = signal(true);

  private token = '';

  form = this.fb.group({
    newPassword: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/)
    ]],
    confirmPassword: ['', Validators.required]
  }, { validators: this.passwordMatchValidator });

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    if (!this.token) {
      this.errorMessage.set('Invalid or missing reset token');
    }
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPassword = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (newPassword && confirmPassword && newPassword.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  togglePassword(): void {
    this.hidePassword.update(hide => !hide);
  }

  toggleConfirmPassword(): void {
    this.hideConfirmPassword.update(hide => !hide);
  }

  onSubmit(): void {
    if (this.form.invalid || !this.token) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    const { newPassword, confirmPassword } = this.form.value;
    this.authService.resetPassword({
      token: this.token,
      newPassword: newPassword!,
      confirmPassword: confirmPassword!
    }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.successMessage.set('Your password has been reset successfully.');
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error?.error?.message || 'An error occurred. Please try again.');
      }
    });
  }
}
