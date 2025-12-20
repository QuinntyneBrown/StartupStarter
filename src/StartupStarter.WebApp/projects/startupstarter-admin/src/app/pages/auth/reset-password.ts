import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
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
    ReactiveFormsModule,
    RouterLink,
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
        <mat-card-header class="reset-password__header">
          <mat-card-title class="text-headline-medium">Set New Password</mat-card-title>
          <mat-card-subtitle>Enter your new password below</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content class="reset-password__content">
          @if (resetSuccess()) {
            <div class="reset-password__success">
              <mat-icon>check_circle</mat-icon>
              <p>Password reset successfully! You can now login with your new password.</p>
              <a mat-raised-button color="primary" routerLink="/login">Go to Login</a>
            </div>
          } @else {
            <form [formGroup]="resetForm" (ngSubmit)="onSubmit()" class="reset-password__form">
              <mat-form-field appearance="outline" class="reset-password__field">
                <mat-label>New Password</mat-label>
                <input matInput [type]="hidePassword() ? 'password' : 'text'" formControlName="password" placeholder="Enter new password">
                <mat-icon matPrefix>lock</mat-icon>
                <button mat-icon-button matSuffix type="button" (click)="togglePasswordVisibility()">
                  <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
                </button>
                @if (resetForm.get('password')?.hasError('required') && resetForm.get('password')?.touched) {
                  <mat-error>Password is required</mat-error>
                }
                @if (resetForm.get('password')?.hasError('minlength') && resetForm.get('password')?.touched) {
                  <mat-error>Password must be at least 8 characters</mat-error>
                }
              </mat-form-field>

              <mat-form-field appearance="outline" class="reset-password__field">
                <mat-label>Confirm Password</mat-label>
                <input matInput [type]="hideConfirmPassword() ? 'password' : 'text'" formControlName="confirmPassword" placeholder="Confirm new password">
                <mat-icon matPrefix>lock</mat-icon>
                <button mat-icon-button matSuffix type="button" (click)="toggleConfirmPasswordVisibility()">
                  <mat-icon>{{ hideConfirmPassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
                </button>
                @if (resetForm.get('confirmPassword')?.hasError('required') && resetForm.get('confirmPassword')?.touched) {
                  <mat-error>Please confirm your password</mat-error>
                }
                @if (resetForm.hasError('passwordMismatch') && resetForm.get('confirmPassword')?.touched) {
                  <mat-error>Passwords do not match</mat-error>
                }
              </mat-form-field>

              @if (errorMessage()) {
                <div class="reset-password__error">
                  <mat-icon>error</mat-icon>
                  <span>{{ errorMessage() }}</span>
                </div>
              }

              <button mat-raised-button color="primary" type="submit" class="reset-password__submit" [disabled]="isLoading()">
                @if (isLoading()) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Reset Password
                }
              </button>
            </form>
          }
        </mat-card-content>

        <mat-card-actions class="reset-password__actions">
          <a mat-button routerLink="/login">
            <mat-icon>arrow_back</mat-icon>
            Back to Login
          </a>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .reset-password {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--spacing-md);
      background: var(--mat-sys-surface-container);

      &__card {
        width: 100%;
        max-width: 400px;
      }

      &__header {
        text-align: center;
        padding-bottom: var(--spacing-lg);
      }

      &__content {
        padding: var(--spacing-md);
      }

      &__form {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
      }

      &__field {
        width: 100%;
      }

      &__success {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: var(--spacing-md);
        padding: var(--spacing-lg);
        text-align: center;
        color: var(--mat-sys-primary);

        mat-icon {
          font-size: 48px;
          width: 48px;
          height: 48px;
        }
      }

      &__error {
        display: flex;
        align-items: center;
        gap: var(--spacing-sm);
        padding: var(--spacing-sm) var(--spacing-md);
        background: var(--mat-sys-error-container);
        color: var(--mat-sys-on-error-container);
        border-radius: var(--radius-sm);
        font: var(--mat-sys-body-small);
      }

      &__submit {
        width: 100%;
        height: 48px;
        margin-top: var(--spacing-sm);
      }

      &__actions {
        display: flex;
        justify-content: center;
        padding: var(--spacing-md);
      }
    }
  `]
})
export class ResetPassword implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly hidePassword = signal(true);
  readonly hideConfirmPassword = signal(true);
  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly resetSuccess = signal(false);

  private token = '';

  resetForm: FormGroup = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: this.passwordMatchValidator });

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'] || '';
    if (!this.token) {
      this.router.navigate(['/login']);
    }
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(v => !v);
  }

  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword.update(v => !v);
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.authService.completePasswordReset({
      token: this.token,
      newPassword: this.resetForm.value.password
    }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.resetSuccess.set(true);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error.error?.message || 'Failed to reset password');
      }
    });
  }
}
