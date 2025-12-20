import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageHeader } from '../../components/shared/page-header';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { AuthService } from '../../services';

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatSelectModule,
    MatTabsModule,
    PageHeader,
    LoadingSpinner
  ],
  template: `
    <app-page-header
      title="Settings"
      subtitle="Manage your account settings"
      icon="settings"
    ></app-page-header>

    @if (isLoading()) {
      <app-loading-spinner message="Loading settings..."></app-loading-spinner>
    } @else {
      <mat-tab-group class="user-settings__tabs">
        <mat-tab label="Preferences">
          <mat-card class="user-settings__card">
            <mat-card-content>
              <form [formGroup]="preferencesForm" class="user-settings__form">
                <mat-form-field appearance="outline" class="user-settings__field">
                  <mat-label>Theme</mat-label>
                  <mat-select formControlName="theme">
                    <mat-option value="light">Light</mat-option>
                    <mat-option value="dark">Dark</mat-option>
                    <mat-option value="system">System Default</mat-option>
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="user-settings__field">
                  <mat-label>Language</mat-label>
                  <mat-select formControlName="language">
                    <mat-option value="en">English</mat-option>
                    <mat-option value="es">Spanish</mat-option>
                    <mat-option value="fr">French</mat-option>
                    <mat-option value="de">German</mat-option>
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="user-settings__field">
                  <mat-label>Timezone</mat-label>
                  <mat-select formControlName="timezone">
                    @for (tz of timezones; track tz) {
                      <mat-option [value]="tz">{{ tz }}</mat-option>
                    }
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="user-settings__field">
                  <mat-label>Date Format</mat-label>
                  <mat-select formControlName="dateFormat">
                    <mat-option value="MM/DD/YYYY">MM/DD/YYYY</mat-option>
                    <mat-option value="DD/MM/YYYY">DD/MM/YYYY</mat-option>
                    <mat-option value="YYYY-MM-DD">YYYY-MM-DD</mat-option>
                  </mat-select>
                </mat-form-field>
              </form>
            </mat-card-content>
            <mat-card-actions align="end">
              <button mat-raised-button color="primary" (click)="savePreferences()" [disabled]="!preferencesForm.valid || isSaving()">
                <mat-icon>save</mat-icon>
                Save Changes
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-tab>

        <mat-tab label="Notifications">
          <mat-card class="user-settings__card">
            <mat-card-content>
              <form [formGroup]="notificationsForm" class="user-settings__form">
                <div class="user-settings__section">
                  <h3 class="text-title-medium">Email Notifications</h3>
                  <mat-slide-toggle formControlName="emailEnabled" class="user-settings__toggle">
                    Enable email notifications
                  </mat-slide-toggle>
                </div>

                @if (notificationsForm.get('emailEnabled')?.value) {
                  <mat-slide-toggle formControlName="emailOnLogin" class="user-settings__toggle">
                    Notify on new login
                  </mat-slide-toggle>

                  <mat-slide-toggle formControlName="emailOnSecurityChange" class="user-settings__toggle">
                    Notify on security changes
                  </mat-slide-toggle>

                  <mat-slide-toggle formControlName="emailOnContentUpdate" class="user-settings__toggle">
                    Notify on content updates
                  </mat-slide-toggle>

                  <mat-slide-toggle formControlName="emailWeeklyDigest" class="user-settings__toggle">
                    Weekly activity digest
                  </mat-slide-toggle>
                }

                <div class="user-settings__section">
                  <h3 class="text-title-medium">Push Notifications</h3>
                  <mat-slide-toggle formControlName="pushEnabled" class="user-settings__toggle">
                    Enable push notifications
                  </mat-slide-toggle>
                </div>
              </form>
            </mat-card-content>
            <mat-card-actions align="end">
              <button mat-raised-button color="primary" (click)="saveNotifications()" [disabled]="!notificationsForm.valid || isSaving()">
                <mat-icon>save</mat-icon>
                Save Changes
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-tab>

        <mat-tab label="Security">
          <mat-card class="user-settings__card">
            <mat-card-content>
              <div class="user-settings__section">
                <h3 class="text-title-medium">Change Password</h3>
                <form [formGroup]="passwordForm" class="user-settings__form">
                  <mat-form-field appearance="outline" class="user-settings__field">
                    <mat-label>Current Password</mat-label>
                    <input matInput formControlName="currentPassword" type="password">
                  </mat-form-field>

                  <mat-form-field appearance="outline" class="user-settings__field">
                    <mat-label>New Password</mat-label>
                    <input matInput formControlName="newPassword" type="password">
                    <mat-hint>At least 12 characters with mixed case, numbers, and symbols</mat-hint>
                  </mat-form-field>

                  <mat-form-field appearance="outline" class="user-settings__field">
                    <mat-label>Confirm New Password</mat-label>
                    <input matInput formControlName="confirmPassword" type="password">
                  </mat-form-field>

                  <button mat-raised-button color="primary" (click)="changePassword()" [disabled]="!passwordForm.valid || isSaving()">
                    <mat-icon>lock</mat-icon>
                    Change Password
                  </button>
                </form>
              </div>

              <div class="user-settings__section">
                <h3 class="text-title-medium">Two-Factor Authentication</h3>
                <p class="text-body-medium user-settings__description">
                  Add an extra layer of security to your account by enabling two-factor authentication.
                </p>
                <button mat-raised-button (click)="setupMfa()">
                  <mat-icon>security</mat-icon>
                  {{ mfaEnabled() ? 'Manage 2FA' : 'Enable 2FA' }}
                </button>
              </div>

              <div class="user-settings__section">
                <h3 class="text-title-medium">Active Sessions</h3>
                <p class="text-body-medium user-settings__description">
                  View and manage your active sessions across devices.
                </p>
                <button mat-raised-button (click)="manageSessions()">
                  <mat-icon>devices</mat-icon>
                  Manage Sessions
                </button>
              </div>
            </mat-card-content>
          </mat-card>
        </mat-tab>
      </mat-tab-group>
    }
  `,
  styles: [`
    .user-settings {
      &__tabs {
        margin-top: var(--spacing-md);
      }

      &__card {
        margin-top: var(--spacing-md);
      }

      &__form {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
        max-width: 500px;
      }

      &__field {
        width: 100%;
      }

      &__toggle {
        margin: var(--spacing-sm) 0;
      }

      &__section {
        padding: var(--spacing-lg) 0;
        border-bottom: 1px solid var(--mat-sys-outline-variant);

        &:first-child {
          padding-top: 0;
        }

        &:last-child {
          border-bottom: none;
        }
      }

      &__description {
        color: var(--mat-sys-on-surface-variant);
        margin: var(--spacing-sm) 0 var(--spacing-md) 0;
      }
    }
  `]
})
export class UserSettings implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly mfaEnabled = signal(false);

  readonly timezones = [
    'UTC',
    'America/New_York',
    'America/Chicago',
    'America/Denver',
    'America/Los_Angeles',
    'Europe/London',
    'Europe/Paris',
    'Asia/Tokyo',
    'Asia/Shanghai',
    'Australia/Sydney'
  ];

  readonly preferencesForm: FormGroup = this.fb.group({
    theme: ['system'],
    language: ['en'],
    timezone: ['UTC'],
    dateFormat: ['MM/DD/YYYY']
  });

  readonly notificationsForm: FormGroup = this.fb.group({
    emailEnabled: [true],
    emailOnLogin: [true],
    emailOnSecurityChange: [true],
    emailOnContentUpdate: [false],
    emailWeeklyDigest: [true],
    pushEnabled: [false]
  });

  readonly passwordForm: FormGroup = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(12)]],
    confirmPassword: ['', Validators.required]
  });

  ngOnInit(): void {
    // Load user settings if available
  }

  savePreferences(): void {
    if (!this.preferencesForm.valid) return;
    this.isSaving.set(true);
    // Save preferences
    setTimeout(() => {
      this.snackBar.open('Preferences saved successfully', 'Close', { duration: 3000 });
      this.isSaving.set(false);
    }, 1000);
  }

  saveNotifications(): void {
    if (!this.notificationsForm.valid) return;
    this.isSaving.set(true);
    // Save notifications
    setTimeout(() => {
      this.snackBar.open('Notification settings saved successfully', 'Close', { duration: 3000 });
      this.isSaving.set(false);
    }, 1000);
  }

  changePassword(): void {
    if (!this.passwordForm.valid) return;

    const { newPassword, confirmPassword } = this.passwordForm.value;
    if (newPassword !== confirmPassword) {
      this.snackBar.open('Passwords do not match', 'Close', { duration: 3000 });
      return;
    }

    this.isSaving.set(true);
    this.authService.changePassword(
      this.passwordForm.value.currentPassword,
      this.passwordForm.value.newPassword
    ).subscribe({
      next: () => {
        this.snackBar.open('Password changed successfully', 'Close', { duration: 3000 });
        this.passwordForm.reset();
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  setupMfa(): void {
    this.snackBar.open('MFA setup coming soon', 'Close', { duration: 3000 });
  }

  manageSessions(): void {
    this.snackBar.open('Session management coming soon', 'Close', { duration: 3000 });
  }
}
