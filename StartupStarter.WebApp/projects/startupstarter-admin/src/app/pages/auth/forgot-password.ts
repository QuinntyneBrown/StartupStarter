import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services';

@Component({
  selector: 'app-forgot-password',
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
    <div class="forgot-password">
      <mat-card class="forgot-password__card">
        <mat-card-header class="forgot-password__header">
          <mat-card-title class="text-headline-medium">Reset Password</mat-card-title>
          <mat-card-subtitle>Enter your email to receive a reset link</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content class="forgot-password__content">
          @if (emailSent()) {
            <div class="forgot-password__success">
              <mat-icon>check_circle</mat-icon>
              <p>Password reset email sent! Check your inbox for further instructions.</p>
            </div>
          } @else {
            <form [formGroup]="resetForm" (ngSubmit)="onSubmit()" class="forgot-password__form">
              <mat-form-field appearance="outline" class="forgot-password__field">
                <mat-label>Email</mat-label>
                <input matInput type="email" formControlName="email" placeholder="Enter your email">
                <mat-icon matPrefix>email</mat-icon>
                @if (resetForm.get('email')?.hasError('required') && resetForm.get('email')?.touched) {
                  <mat-error>Email is required</mat-error>
                }
                @if (resetForm.get('email')?.hasError('email') && resetForm.get('email')?.touched) {
                  <mat-error>Please enter a valid email</mat-error>
                }
              </mat-form-field>

              @if (errorMessage()) {
                <div class="forgot-password__error">
                  <mat-icon>error</mat-icon>
                  <span>{{ errorMessage() }}</span>
                </div>
              }

              <button mat-raised-button color="primary" type="submit" class="forgot-password__submit" [disabled]="isLoading()">
                @if (isLoading()) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  Send Reset Link
                }
              </button>
            </form>
          }
        </mat-card-content>

        <mat-card-actions class="forgot-password__actions">
          <a mat-button routerLink="/login">
            <mat-icon>arrow_back</mat-icon>
            Back to Login
          </a>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .forgot-password {
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
export class ForgotPassword {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly emailSent = signal(false);

  resetForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  onSubmit(): void {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.authService.requestPasswordReset({ email: this.resetForm.value.email }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.emailSent.set(true);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error.error?.message || 'Failed to send reset email');
      }
    });
  }
}
