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
import { StatusBadge } from '../../components/shared/status-badge';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { EmptyState } from '../../components/shared/empty-state';
import { ConfirmDialog } from '../../components/shared/confirm-dialog';
import { UserService } from '../../services';
import { User, UserStatus } from '../../models';

@Component({
  selector: 'app-user-list',
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
    StatusBadge,
    LoadingSpinner,
    EmptyState
  ],
  template: `
    <app-page-header
      title="Users"
      subtitle="Manage system users"
      icon="people"
    >
      <button mat-raised-button color="primary" routerLink="/users/invite">
        <mat-icon>person_add</mat-icon>
        Invite User
      </button>
    </app-page-header>

    <mat-card class="user-list__card">
      <mat-card-content>
        <div class="user-list__toolbar">
          <mat-form-field appearance="outline" class="user-list__search">
            <mat-label>Search users</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name or email...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading users..."></app-loading-spinner>
        } @else if (filteredUsers().length === 0) {
          <app-empty-state
            icon="people"
            title="No users found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by inviting your first user'"
            [actionLabel]="searchQuery() ? undefined : 'Invite User'"
            actionIcon="person_add"
            (action)="router.navigate(['/users/invite'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredUsers()" class="user-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let user">
                <a [routerLink]="['/users', user.userId]" class="user-list__link">
                  {{ user.firstName }} {{ user.lastName }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>Email</th>
              <td mat-cell *matCellDef="let user">{{ user.email }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let user">
                <app-status-badge
                  [status]="getStatusType(user.status)"
                  [label]="user.status"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="created">
              <th mat-header-cell *matHeaderCellDef>Created</th>
              <td mat-cell *matCellDef="let user">{{ formatDate(user.createdAt) }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let user">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/users', user.userId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View Details</span>
                  </button>
                  @if (user.status === 'Active') {
                    <button mat-menu-item (click)="deactivateUser(user)">
                      <mat-icon>person_off</mat-icon>
                      <span>Deactivate</span>
                    </button>
                    <button mat-menu-item (click)="lockUser(user)">
                      <mat-icon>lock</mat-icon>
                      <span>Lock</span>
                    </button>
                  } @else if (user.status === 'Inactive') {
                    <button mat-menu-item (click)="activateUser(user)">
                      <mat-icon>person</mat-icon>
                      <span>Activate</span>
                    </button>
                  } @else if (user.status === 'Locked') {
                    <button mat-menu-item (click)="unlockUser(user)">
                      <mat-icon>lock_open</mat-icon>
                      <span>Unlock</span>
                    </button>
                  }
                  <button mat-menu-item (click)="deleteUser(user)" class="user-list__delete">
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
    .user-list {
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
export class UserList implements OnInit {
  readonly router = inject(Router);
  private readonly userService = inject(UserService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly users = signal<User[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'email', 'status', 'created', 'actions'];

  readonly filteredUsers = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.users();
    return this.users().filter(u =>
      u.firstName.toLowerCase().includes(query) ||
      u.lastName.toLowerCase().includes(query) ||
      u.email.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users.set(users);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStatusType(status: UserStatus): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (status) {
      case UserStatus.Active: return 'success';
      case UserStatus.Invited: return 'info';
      case UserStatus.Inactive: return 'neutral';
      case UserStatus.Locked: return 'warning';
      case UserStatus.Deleted: return 'error';
      default: return 'neutral';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }

  activateUser(user: User): void {
    this.userService.activate(user.userId).subscribe({
      next: () => {
        this.snackBar.open('User activated successfully', 'Close', { duration: 3000 });
        this.loadUsers();
      }
    });
  }

  deactivateUser(user: User): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Deactivate User',
        message: `Are you sure you want to deactivate ${user.firstName} ${user.lastName}?`,
        confirmText: 'Deactivate',
        confirmColor: 'warn',
        icon: 'person_off'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.userService.deactivate(user.userId, 'Deactivated by admin').subscribe({
          next: () => {
            this.snackBar.open('User deactivated successfully', 'Close', { duration: 3000 });
            this.loadUsers();
          }
        });
      }
    });
  }

  lockUser(user: User): void {
    this.userService.lock(user.userId, 'Locked by admin').subscribe({
      next: () => {
        this.snackBar.open('User locked successfully', 'Close', { duration: 3000 });
        this.loadUsers();
      }
    });
  }

  unlockUser(user: User): void {
    this.userService.unlock(user.userId).subscribe({
      next: () => {
        this.snackBar.open('User unlocked successfully', 'Close', { duration: 3000 });
        this.loadUsers();
      }
    });
  }

  deleteUser(user: User): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete User',
        message: `Are you sure you want to delete ${user.firstName} ${user.lastName}? This action cannot be undone.`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.userService.delete(user.userId).subscribe({
          next: () => {
            this.snackBar.open('User deleted successfully', 'Close', { duration: 3000 });
            this.loadUsers();
          }
        });
      }
    });
  }
}
