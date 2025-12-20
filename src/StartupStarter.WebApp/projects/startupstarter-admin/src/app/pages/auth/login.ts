import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
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
    <div class="login">
      <mat-card class="login__card">
        <mat-card-header class="login__header">
          <mat-card-title class="text-headline-medium">StartupStarter Admin</mat-card-title>
          <mat-card-subtitle>Sign in to your account</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content class="login__content">
          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="login__form">
            <mat-form-field appearance="outline" class="login__field">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" placeholder="Enter your email">
              <mat-icon matPrefix>email</mat-icon>
              @if (loginForm.get('email')?.hasError('required') && loginForm.get('email')?.touched) {
                <mat-error>Email is required</mat-error>
              }
              @if (loginForm.get('email')?.hasError('email') && loginForm.get('email')?.touched) {
                <mat-error>Please enter a valid email</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="login__field">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword() ? 'password' : 'text'" formControlName="password" placeholder="Enter your password">
              <mat-icon matPrefix>lock</mat-icon>
              <button mat-icon-button matSuffix type="button" (click)="togglePasswordVisibility()">
                <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
              @if (loginForm.get('password')?.hasError('required') && loginForm.get('password')?.touched) {
                <mat-error>Password is required</mat-error>
              }
            </mat-form-field>

            @if (errorMessage()) {
              <div class="login__error">
                <mat-icon>error</mat-icon>
                <span>{{ errorMessage() }}</span>
              </div>
            }

            <button mat-raised-button color="primary" type="submit" class="login__submit" [disabled]="isLoading()">
              @if (isLoading()) {
                <mat-spinner diameter="20"></mat-spinner>
              } @else {
                Sign In
              }
            </button>
          </form>
        </mat-card-content>

        <mat-card-actions class="login__actions">
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
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly hidePassword = signal(true);
  readonly isLoading = signal(false);
  readonly errorMessage = signal('');

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  togglePasswordVisibility(): void {
    this.hidePassword.update(v => !v);
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error.error?.message || 'Invalid email or password');
      }
    });
  }
}
