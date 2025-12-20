import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent, BadgeType } from '../../components/shared';
import { UserService } from '../../services';
import { User, UserStatus } from '../../models';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    PageHeaderComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    StatusBadgeComponent
  ],
  template: `
    <app-page-header title="Users" subtitle="Manage user accounts" icon="people">
      <button mat-flat-button color="primary" (click)="invite()">
        <mat-icon>person_add</mat-icon>
        Invite User
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search users</mat-label>
          <input matInput [(ngModel)]="searchQuery" placeholder="Search by name or email...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        @if (isLoading()) {
          <app-loading-spinner message="Loading users..."></app-loading-spinner>
        } @else if (filtered().length === 0) {
          <app-empty-state
            icon="people"
            title="No users found"
            [message]="searchQuery ? 'Try adjusting your search' : 'Invite your first user to get started'"
            [actionLabel]="searchQuery ? '' : 'Invite User'"
            (action)="invite()">
          </app-empty-state>
        } @else {
          <table mat-table [dataSource]="filtered()" class="full-width">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let user">{{ user.firstName }} {{ user.lastName }}</td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>Email</th>
              <td mat-cell *matCellDef="let user">{{ user.email }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let user">
                <app-status-badge [label]="user.status" [type]="getStatusType(user.status)"></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let user">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="edit(user)">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  @if (user.status === 'Inactive') {
                    <button mat-menu-item (click)="activate(user)">
                      <mat-icon>check_circle</mat-icon>
                      <span>Activate</span>
                    </button>
                  }
                  @if (user.status === 'Active') {
                    <button mat-menu-item (click)="deactivate(user)">
                      <mat-icon>block</mat-icon>
                      <span>Deactivate</span>
                    </button>
                  }
                  @if (user.status === 'Locked') {
                    <button mat-menu-item (click)="unlock(user)">
                      <mat-icon>lock_open</mat-icon>
                      <span>Unlock</span>
                    </button>
                  } @else if (user.status !== 'Locked') {
                    <button mat-menu-item (click)="lock(user)">
                      <mat-icon>lock</mat-icon>
                      <span>Lock</span>
                    </button>
                  }
                  <button mat-menu-item (click)="delete(user)">
                    <mat-icon color="warn">delete</mat-icon>
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
    .search-field { width: 100%; max-width: 400px; margin-bottom: 16px; }
    .full-width { width: 100%; }
  `]
})
export class UserListComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly users = signal<User[]>([]);
  searchQuery = '';
  displayedColumns = ['name', 'email', 'status', 'actions'];

  readonly filtered = computed(() => {
    const query = this.searchQuery.toLowerCase();
    return this.users().filter(u =>
      u.firstName.toLowerCase().includes(query) ||
      u.lastName.toLowerCase().includes(query) ||
      u.email.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.userService.getAll().subscribe({
      next: (users) => { this.users.set(users); this.isLoading.set(false); },
      error: () => { this.isLoading.set(false); }
    });
  }

  getStatusType(status: UserStatus): BadgeType {
    switch (status) {
      case UserStatus.Active: return 'success';
      case UserStatus.Inactive: return 'neutral';
      case UserStatus.Locked: return 'error';
      case UserStatus.Invited: return 'info';
      case UserStatus.Deleted: return 'error';
      default: return 'neutral';
    }
  }

  invite(): void { this.router.navigate(['/users/invite']); }
  edit(user: User): void { this.router.navigate(['/users', user.userId]); }

  activate(user: User): void {
    this.userService.activate(user.userId).subscribe(() => this.load());
  }

  deactivate(user: User): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Deactivate User', message: `Deactivate ${user.firstName} ${user.lastName}?`, confirmText: 'Deactivate', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.userService.deactivate(user.userId, { reason: 'Manual deactivation' }).subscribe(() => this.load());
      }
    });
  }

  lock(user: User): void {
    this.userService.lock(user.userId, { reason: 'Manual lock' }).subscribe(() => this.load());
  }

  unlock(user: User): void {
    this.userService.unlock(user.userId).subscribe(() => this.load());
  }

  delete(user: User): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete User', message: `Delete ${user.firstName} ${user.lastName}? This action cannot be undone.`, confirmText: 'Delete', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.userService.delete(user.userId).subscribe(() => this.load());
      }
    });
  }
}
