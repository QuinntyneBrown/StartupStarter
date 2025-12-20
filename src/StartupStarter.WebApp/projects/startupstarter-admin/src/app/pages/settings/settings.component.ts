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
  template: `
    <app-page-header title="Settings" subtitle="Manage your account settings" icon="settings"></app-page-header>

    <div class="settings-grid">
      <mat-card>
        <mat-card-header><mat-card-title>Profile</mat-card-title></mat-card-header>
        <mat-card-content>
          <form [formGroup]="profileForm" (ngSubmit)="saveProfile()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>First Name</mat-label>
              <input matInput formControlName="firstName">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Last Name</mat-label>
              <input matInput formControlName="lastName">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" readonly>
            </mat-form-field>
            <button mat-flat-button color="primary" type="submit" [disabled]="profileForm.invalid">Save Changes</button>
          </form>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-header><mat-card-title>Security</mat-card-title></mat-card-header>
        <mat-card-content>
          <div class="security-item">
            <div>
              <strong>Two-Factor Authentication</strong>
              <p>Add an extra layer of security to your account</p>
            </div>
            <mat-slide-toggle [checked]="mfaEnabled()" (change)="toggleMfa($event.checked)"></mat-slide-toggle>
          </div>
          <mat-divider></mat-divider>
          <h4>Change Password</h4>
          <form [formGroup]="passwordForm" (ngSubmit)="changePassword()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Current Password</mat-label>
              <input matInput type="password" formControlName="currentPassword">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>New Password</mat-label>
              <input matInput type="password" formControlName="newPassword">
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Confirm Password</mat-label>
              <input matInput type="password" formControlName="confirmPassword">
            </mat-form-field>
            <button mat-flat-button color="primary" type="submit" [disabled]="passwordForm.invalid">Change Password</button>
          </form>
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-header><mat-card-title>Active Sessions</mat-card-title></mat-card-header>
        <mat-card-content>
          @if (sessions().length === 0) {
            <p>No active sessions</p>
          } @else {
            @for (session of sessions(); track session.sessionId) {
              <div class="session-item">
                <div>
                  <strong>{{ session.userAgent }}</strong>
                  <p>{{ session.ipAddress }} - {{ session.createdAt | date:'short' }}</p>
                </div>
                <button mat-icon-button color="warn" (click)="revokeSession(session.sessionId)">
                  <mat-icon>logout</mat-icon>
                </button>
              </div>
            }
          }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .settings-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(400px, 1fr)); gap: 24px; }
    .full-width { width: 100%; }
    .security-item { display: flex; justify-content: space-between; align-items: center; padding: 16px 0; }
    .security-item p { color: rgba(0,0,0,0.6); margin: 4px 0 0; }
    .session-item { display: flex; justify-content: space-between; align-items: center; padding: 12px 0; border-bottom: 1px solid rgba(0,0,0,0.1); }
    .session-item p { color: rgba(0,0,0,0.6); margin: 4px 0 0; font-size: 12px; }
    h4 { margin: 24px 0 16px; }
    mat-divider { margin: 16px 0; }
  `]
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
