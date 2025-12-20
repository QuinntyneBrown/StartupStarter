import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageHeader } from '../../components/shared/page-header';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { AuthService, ProfileService } from '../../services';
import { Profile } from '../../models';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    PageHeader,
    LoadingSpinner
  ],
  template: `
    <app-page-header
      title="My Profile"
      subtitle="View and update your profile information"
      icon="account_circle"
    ></app-page-header>

    @if (isLoading()) {
      <app-loading-spinner message="Loading profile..."></app-loading-spinner>
    } @else {
      <div class="user-profile__container">
        <mat-card class="user-profile__card">
          <mat-card-content>
            <div class="user-profile__header">
              <div class="user-profile__avatar">
                @if (profile()?.avatarUrl) {
                  <img [src]="profile()?.avatarUrl" [alt]="profile()?.firstName">
                } @else {
                  <span class="user-profile__initials">
                    {{ getInitials() }}
                  </span>
                }
                <button mat-mini-fab class="user-profile__avatar-edit" (click)="uploadAvatar()">
                  <mat-icon>camera_alt</mat-icon>
                </button>
              </div>
              <div class="user-profile__info">
                <h2 class="text-headline-medium">{{ profile()?.firstName }} {{ profile()?.lastName }}</h2>
                <p class="text-body-medium">{{ profile()?.email }}</p>
              </div>
            </div>

            <form [formGroup]="profileForm" class="user-profile__form">
              <div class="user-profile__row">
                <mat-form-field appearance="outline" class="user-profile__field">
                  <mat-label>First Name</mat-label>
                  <input matInput formControlName="firstName">
                </mat-form-field>

                <mat-form-field appearance="outline" class="user-profile__field">
                  <mat-label>Last Name</mat-label>
                  <input matInput formControlName="lastName">
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline" class="user-profile__field--full">
                <mat-label>Email</mat-label>
                <input matInput formControlName="email" type="email">
              </mat-form-field>

              <mat-form-field appearance="outline" class="user-profile__field--full">
                <mat-label>Phone Number</mat-label>
                <input matInput formControlName="phoneNumber" type="tel">
              </mat-form-field>

              <mat-form-field appearance="outline" class="user-profile__field--full">
                <mat-label>Bio</mat-label>
                <textarea matInput formControlName="bio" rows="3"></textarea>
              </mat-form-field>
            </form>
          </mat-card-content>
          <mat-card-actions align="end">
            <button mat-button (click)="resetForm()">Reset</button>
            <button mat-raised-button color="primary" (click)="saveProfile()" [disabled]="!profileForm.valid || isSaving()">
              <mat-icon>save</mat-icon>
              Save Changes
            </button>
          </mat-card-actions>
        </mat-card>

        <mat-card class="user-profile__card">
          <mat-card-header>
            <mat-card-title>Account Information</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="user-profile__account-info">
              <div class="user-profile__info-row">
                <span class="user-profile__label">Account Status</span>
                <span class="user-profile__value user-profile__value--active">Active</span>
              </div>
              <div class="user-profile__info-row">
                <span class="user-profile__label">Member Since</span>
                <span class="user-profile__value">{{ formatDate(profile()?.createdAt) }}</span>
              </div>
              <div class="user-profile__info-row">
                <span class="user-profile__label">Last Updated</span>
                <span class="user-profile__value">{{ formatDate(profile()?.updatedAt) }}</span>
              </div>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    }
  `,
  styles: [`
    .user-profile {
      &__container {
        display: grid;
        gap: var(--spacing-lg);
        grid-template-columns: 1fr;

        @media (min-width: 1024px) {
          grid-template-columns: 2fr 1fr;
        }
      }

      &__card {
        height: fit-content;
      }

      &__header {
        display: flex;
        align-items: center;
        gap: var(--spacing-lg);
        margin-bottom: var(--spacing-xl);
        padding-bottom: var(--spacing-lg);
        border-bottom: 1px solid var(--mat-sys-outline-variant);
      }

      &__avatar {
        position: relative;
        width: 100px;
        height: 100px;
        border-radius: 50%;
        overflow: hidden;
        background: var(--mat-sys-primary-container);
        display: flex;
        align-items: center;
        justify-content: center;

        img {
          width: 100%;
          height: 100%;
          object-fit: cover;
        }
      }

      &__initials {
        font-size: 36px;
        font-weight: 500;
        color: var(--mat-sys-on-primary-container);
      }

      &__avatar-edit {
        position: absolute;
        bottom: 0;
        right: 0;
        transform: scale(0.8);
      }

      &__info {
        h2 {
          margin: 0 0 var(--spacing-xs) 0;
        }

        p {
          margin: 0;
          color: var(--mat-sys-on-surface-variant);
        }
      }

      &__form {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
      }

      &__row {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: var(--spacing-md);
      }

      &__field {
        width: 100%;

        &--full {
          width: 100%;
        }
      }

      &__account-info {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
      }

      &__info-row {
        display: flex;
        justify-content: space-between;
        padding: var(--spacing-sm) 0;
        border-bottom: 1px solid var(--mat-sys-outline-variant);

        &:last-child {
          border-bottom: none;
        }
      }

      &__label {
        color: var(--mat-sys-on-surface-variant);
      }

      &__value {
        font-weight: 500;

        &--active {
          color: var(--mat-sys-primary);
        }
      }
    }
  `]
})
export class UserProfile implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly profileService = inject(ProfileService);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly profile = signal<Profile | null>(null);

  readonly profileForm: FormGroup = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: [''],
    bio: ['']
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  private loadProfile(): void {
    const currentUser = this.authService.currentUser();
    if (currentUser?.userId) {
      this.profileService.getByUserId(currentUser.userId).subscribe({
        next: (profile) => {
          this.profile.set(profile);
          this.profileForm.patchValue({
            firstName: profile.firstName,
            lastName: profile.lastName,
            email: profile.email,
            phoneNumber: profile.phoneNumber || '',
            bio: profile.bio || ''
          });
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
    } else {
      this.isLoading.set(false);
    }
  }

  getInitials(): string {
    const profile = this.profile();
    if (!profile) return '';
    return `${profile.firstName.charAt(0)}${profile.lastName.charAt(0)}`.toUpperCase();
  }

  formatDate(date: Date | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString();
  }

  uploadAvatar(): void {
    this.snackBar.open('Avatar upload coming soon', 'Close', { duration: 3000 });
  }

  resetForm(): void {
    const profile = this.profile();
    if (profile) {
      this.profileForm.patchValue({
        firstName: profile.firstName,
        lastName: profile.lastName,
        email: profile.email,
        phoneNumber: profile.phoneNumber || '',
        bio: profile.bio || ''
      });
    }
  }

  saveProfile(): void {
    if (!this.profileForm.valid) return;
    const profile = this.profile();
    if (!profile) return;

    this.isSaving.set(true);
    this.profileService.update(profile.profileId, this.profileForm.value).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.snackBar.open('Profile updated successfully', 'Close', { duration: 3000 });
        this.isSaving.set(false);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }
}
