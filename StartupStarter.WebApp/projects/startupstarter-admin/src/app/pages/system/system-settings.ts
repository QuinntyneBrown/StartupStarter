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
import { SystemService } from '../../services';
import { SystemConfig } from '../../models';

@Component({
  selector: 'app-system-settings',
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
      title="System Settings"
      subtitle="Configure system-wide settings"
      icon="settings"
    ></app-page-header>

    @if (isLoading()) {
      <app-loading-spinner message="Loading settings..."></app-loading-spinner>
    } @else {
      <mat-tab-group class="system-settings__tabs">
        <mat-tab label="General">
          <mat-card class="system-settings__card">
            <mat-card-content>
              <form [formGroup]="generalForm" class="system-settings__form">
                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Application Name</mat-label>
                  <input matInput formControlName="applicationName">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Application URL</mat-label>
                  <input matInput formControlName="applicationUrl">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Support Email</mat-label>
                  <input matInput formControlName="supportEmail" type="email">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Default Timezone</mat-label>
                  <mat-select formControlName="defaultTimezone">
                    @for (tz of timezones; track tz) {
                      <mat-option [value]="tz">{{ tz }}</mat-option>
                    }
                  </mat-select>
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Default Language</mat-label>
                  <mat-select formControlName="defaultLanguage">
                    <mat-option value="en">English</mat-option>
                    <mat-option value="es">Spanish</mat-option>
                    <mat-option value="fr">French</mat-option>
                    <mat-option value="de">German</mat-option>
                  </mat-select>
                </mat-form-field>
              </form>
            </mat-card-content>
            <mat-card-actions align="end">
              <button mat-raised-button color="primary" (click)="saveGeneralSettings()" [disabled]="!generalForm.valid || isSaving()">
                <mat-icon>save</mat-icon>
                Save Changes
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-tab>

        <mat-tab label="Security">
          <mat-card class="system-settings__card">
            <mat-card-content>
              <form [formGroup]="securityForm" class="system-settings__form">
                <mat-slide-toggle formControlName="requireMfa" class="system-settings__toggle">
                  Require Multi-Factor Authentication
                </mat-slide-toggle>

                <mat-slide-toggle formControlName="enforcePasswordPolicy" class="system-settings__toggle">
                  Enforce Password Policy
                </mat-slide-toggle>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Minimum Password Length</mat-label>
                  <input matInput formControlName="minPasswordLength" type="number" min="8" max="32">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Session Timeout (minutes)</mat-label>
                  <input matInput formControlName="sessionTimeout" type="number" min="5" max="1440">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Max Login Attempts</mat-label>
                  <input matInput formControlName="maxLoginAttempts" type="number" min="3" max="10">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>Lockout Duration (minutes)</mat-label>
                  <input matInput formControlName="lockoutDuration" type="number" min="5" max="60">
                </mat-form-field>
              </form>
            </mat-card-content>
            <mat-card-actions align="end">
              <button mat-raised-button color="primary" (click)="saveSecuritySettings()" [disabled]="!securityForm.valid || isSaving()">
                <mat-icon>save</mat-icon>
                Save Changes
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-tab>

        <mat-tab label="Email">
          <mat-card class="system-settings__card">
            <mat-card-content>
              <form [formGroup]="emailForm" class="system-settings__form">
                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>SMTP Host</mat-label>
                  <input matInput formControlName="smtpHost">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>SMTP Port</mat-label>
                  <input matInput formControlName="smtpPort" type="number">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>SMTP Username</mat-label>
                  <input matInput formControlName="smtpUsername">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>SMTP Password</mat-label>
                  <input matInput formControlName="smtpPassword" type="password">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>From Email</mat-label>
                  <input matInput formControlName="fromEmail" type="email">
                </mat-form-field>

                <mat-form-field appearance="outline" class="system-settings__field">
                  <mat-label>From Name</mat-label>
                  <input matInput formControlName="fromName">
                </mat-form-field>

                <mat-slide-toggle formControlName="enableSsl" class="system-settings__toggle">
                  Enable SSL/TLS
                </mat-slide-toggle>
              </form>
            </mat-card-content>
            <mat-card-actions align="end">
              <button mat-button (click)="testEmailSettings()" [disabled]="isSaving()">
                <mat-icon>send</mat-icon>
                Send Test Email
              </button>
              <button mat-raised-button color="primary" (click)="saveEmailSettings()" [disabled]="!emailForm.valid || isSaving()">
                <mat-icon>save</mat-icon>
                Save Changes
              </button>
            </mat-card-actions>
          </mat-card>
        </mat-tab>

        <mat-tab label="Maintenance">
          <mat-card class="system-settings__card">
            <mat-card-content>
              <div class="system-settings__section">
                <h3 class="text-title-medium">Maintenance Mode</h3>
                <p class="text-body-medium system-settings__description">
                  Enable maintenance mode to prevent users from accessing the application.
                </p>
                <mat-slide-toggle [checked]="maintenanceMode()" (change)="toggleMaintenanceMode($event.checked)">
                  {{ maintenanceMode() ? 'Maintenance Mode Enabled' : 'Maintenance Mode Disabled' }}
                </mat-slide-toggle>
              </div>

              <div class="system-settings__section">
                <h3 class="text-title-medium">Cache Management</h3>
                <p class="text-body-medium system-settings__description">
                  Clear application caches to resolve issues or apply configuration changes.
                </p>
                <button mat-raised-button (click)="clearCache()" [disabled]="isSaving()">
                  <mat-icon>cached</mat-icon>
                  Clear Cache
                </button>
              </div>

              <div class="system-settings__section">
                <h3 class="text-title-medium">System Health</h3>
                <p class="text-body-medium system-settings__description">
                  Check the health status of system services.
                </p>
                <button mat-raised-button (click)="checkHealth()" [disabled]="isSaving()">
                  <mat-icon>health_and_safety</mat-icon>
                  Run Health Check
                </button>
              </div>
            </mat-card-content>
          </mat-card>
        </mat-tab>
      </mat-tab-group>
    }
  `,
  styles: [`
    .system-settings {
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
export class SystemSettings implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly systemService = inject(SystemService);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly maintenanceMode = signal(false);

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

  readonly generalForm: FormGroup = this.fb.group({
    applicationName: ['', Validators.required],
    applicationUrl: ['', Validators.required],
    supportEmail: ['', [Validators.required, Validators.email]],
    defaultTimezone: ['UTC'],
    defaultLanguage: ['en']
  });

  readonly securityForm: FormGroup = this.fb.group({
    requireMfa: [false],
    enforcePasswordPolicy: [true],
    minPasswordLength: [12, [Validators.min(8), Validators.max(32)]],
    sessionTimeout: [30, [Validators.min(5), Validators.max(1440)]],
    maxLoginAttempts: [5, [Validators.min(3), Validators.max(10)]],
    lockoutDuration: [15, [Validators.min(5), Validators.max(60)]]
  });

  readonly emailForm: FormGroup = this.fb.group({
    smtpHost: [''],
    smtpPort: [587],
    smtpUsername: [''],
    smtpPassword: [''],
    fromEmail: ['', Validators.email],
    fromName: [''],
    enableSsl: [true]
  });

  ngOnInit(): void {
    this.loadSettings();
  }

  private loadSettings(): void {
    this.systemService.getSettings().subscribe({
      next: (settings) => {
        if (settings) {
          this.generalForm.patchValue({
            applicationName: settings.applicationName,
            applicationUrl: settings.applicationUrl,
            supportEmail: settings.supportEmail,
            defaultTimezone: settings.defaultTimezone,
            defaultLanguage: settings.defaultLanguage
          });
          this.maintenanceMode.set(settings.maintenanceMode);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  saveGeneralSettings(): void {
    if (!this.generalForm.valid) return;
    this.isSaving.set(true);
    this.systemService.updateSettings(this.generalForm.value).subscribe({
      next: () => {
        this.snackBar.open('General settings saved successfully', 'Close', { duration: 3000 });
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  saveSecuritySettings(): void {
    if (!this.securityForm.valid) return;
    this.isSaving.set(true);
    this.systemService.updateSettings(this.securityForm.value).subscribe({
      next: () => {
        this.snackBar.open('Security settings saved successfully', 'Close', { duration: 3000 });
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  saveEmailSettings(): void {
    if (!this.emailForm.valid) return;
    this.isSaving.set(true);
    this.systemService.updateSettings(this.emailForm.value).subscribe({
      next: () => {
        this.snackBar.open('Email settings saved successfully', 'Close', { duration: 3000 });
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  testEmailSettings(): void {
    this.isSaving.set(true);
    this.snackBar.open('Test email sent', 'Close', { duration: 3000 });
    this.isSaving.set(false);
  }

  toggleMaintenanceMode(enabled: boolean): void {
    this.systemService.setMaintenanceMode(enabled).subscribe({
      next: () => {
        this.maintenanceMode.set(enabled);
        this.snackBar.open(
          enabled ? 'Maintenance mode enabled' : 'Maintenance mode disabled',
          'Close',
          { duration: 3000 }
        );
      }
    });
  }

  clearCache(): void {
    this.isSaving.set(true);
    this.systemService.clearCache().subscribe({
      next: () => {
        this.snackBar.open('Cache cleared successfully', 'Close', { duration: 3000 });
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  checkHealth(): void {
    this.isSaving.set(true);
    this.systemService.getHealth().subscribe({
      next: (health) => {
        this.snackBar.open(`System status: ${health.status}`, 'Close', { duration: 5000 });
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
