import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageHeaderComponent } from '../../components/shared';
import { AuthService } from '../../services';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatSlideToggleModule, MatDividerModule, PageHeaderComponent
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  readonly mfaEnabled = signal(false);
  readonly sessions = signal<Array<{ sessionId: string; userAgent: string; ipAddress: string; createdAt: Date }>>([]);

  profileForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: [{ value: '', disabled: true }]
  });

  passwordForm = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  });

  ngOnInit(): void {
    const user = this.authService.user();
    if (user) {
      this.profileForm.patchValue({ firstName: user.firstName, lastName: user.lastName, email: user.email });
    }
    this.loadSessions();
  }

  loadSessions(): void {
    this.authService.getSessions().subscribe({
      next: (sessions) => this.sessions.set(sessions as any)
    });
  }

  saveProfile(): void {
    this.snackBar.open('Profile updated', 'Close', { duration: 3000 });
  }

  changePassword(): void {
    if (this.passwordForm.invalid) return;
    const { currentPassword, newPassword, confirmPassword } = this.passwordForm.value;
    this.authService.changePassword({ currentPassword: currentPassword!, newPassword: newPassword!, confirmPassword: confirmPassword! }).subscribe({
      next: () => {
        this.snackBar.open('Password changed successfully', 'Close', { duration: 3000 });
        this.passwordForm.reset();
      },
      error: () => this.snackBar.open('Failed to change password', 'Close', { duration: 3000 })
    });
  }

  toggleMfa(enabled: boolean): void {
    if (enabled) {
      this.authService.enableMfa().subscribe({ next: () => this.mfaEnabled.set(true) });
    } else {
      this.authService.disableMfa().subscribe({ next: () => this.mfaEnabled.set(false) });
    }
  }

  revokeSession(sessionId: string): void {
    this.authService.revokeSession(sessionId).subscribe(() => this.loadSessions());
  }
}
