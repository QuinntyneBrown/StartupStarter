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
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
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
