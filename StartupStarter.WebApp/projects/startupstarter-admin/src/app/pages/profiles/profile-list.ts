import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { PageHeader } from '../../components/shared/page-header';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { EmptyState } from '../../components/shared/empty-state';
import { ConfirmDialog } from '../../components/shared/confirm-dialog';
import { ProfileService } from '../../services';
import { Profile } from '../../models';

@Component({
  selector: 'app-profile-list',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    PageHeader,
    LoadingSpinner,
    EmptyState
  ],
  template: `
    <app-page-header
      title="Profiles"
      subtitle="Manage user profiles"
      icon="person"
    >
      <button mat-raised-button color="primary" routerLink="/profiles/new">
        <mat-icon>add</mat-icon>
        New Profile
      </button>
    </app-page-header>

    <mat-card class="profile-list__card">
      <mat-card-content>
        <div class="profile-list__toolbar">
          <mat-form-field appearance="outline" class="profile-list__search">
            <mat-label>Search profiles</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading profiles..."></app-loading-spinner>
        } @else if (filteredProfiles().length === 0) {
          <app-empty-state
            icon="person"
            title="No profiles found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by creating your first profile'"
            [actionLabel]="searchQuery() ? undefined : 'Create Profile'"
            actionIcon="add"
            (action)="router.navigate(['/profiles/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredProfiles()" class="profile-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let profile">
                <div class="profile-list__name">
                  <span class="profile-list__avatar">
                    {{ getInitials(profile.firstName, profile.lastName) }}
                  </span>
                  <a [routerLink]="['/profiles', profile.profileId]" class="profile-list__link">
                    {{ profile.firstName }} {{ profile.lastName }}
                  </a>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>Email</th>
              <td mat-cell *matCellDef="let profile">{{ profile.email }}</td>
            </ng-container>

            <ng-container matColumnDef="phone">
              <th mat-header-cell *matHeaderCellDef>Phone</th>
              <td mat-cell *matCellDef="let profile">{{ profile.phoneNumber || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let profile">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/profiles', profile.profileId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View</span>
                  </button>
                  <button mat-menu-item [routerLink]="['/profiles', profile.profileId, 'edit']">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  <button mat-menu-item (click)="deleteProfile(profile)" class="profile-list__delete">
                    <mat-icon>delete</mat-icon>
                    <span>Delete</span>
                  </button>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .profile-list {
      &__card {
        overflow: hidden;
      }

      &__toolbar {
        display: flex;
        gap: var(--spacing-md);
        margin-bottom: var(--spacing-md);
      }

      &__search {
        flex: 1;
        max-width: 400px;
      }

      &__table {
        width: 100%;
      }

      &__name {
        display: flex;
        align-items: center;
        gap: var(--spacing-sm);
      }

      &__avatar {
        width: 32px;
        height: 32px;
        border-radius: 50%;
        background: var(--mat-sys-primary-container);
        color: var(--mat-sys-on-primary-container);
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 12px;
        font-weight: 500;
      }

      &__link {
        color: var(--mat-sys-primary);
        text-decoration: none;
        font-weight: 500;

        &:hover {
          text-decoration: underline;
        }
      }

      &__delete {
        color: var(--mat-sys-error);
      }
    }
  `]
})
export class ProfileList implements OnInit {
  readonly router = inject(Router);
  private readonly profileService = inject(ProfileService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly profiles = signal<Profile[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'email', 'phone', 'actions'];

  readonly filteredProfiles = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.profiles();
    return this.profiles().filter(p =>
      `${p.firstName} ${p.lastName}`.toLowerCase().includes(query) ||
      p.email.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadProfiles();
  }

  private loadProfiles(): void {
    this.profileService.getAll().subscribe({
      next: (profiles) => {
        this.profiles.set(profiles);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getInitials(firstName: string, lastName: string): string {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  }

  deleteProfile(profile: Profile): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Profile',
        message: `Are you sure you want to delete the profile for "${profile.firstName} ${profile.lastName}"?`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.profileService.delete(profile.profileId).subscribe({
          next: () => {
            this.snackBar.open('Profile deleted successfully', 'Close', { duration: 3000 });
            this.loadProfiles();
          }
        });
      }
    });
  }
}
