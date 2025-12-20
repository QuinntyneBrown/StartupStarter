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
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent, BadgeType } from '../../../components/shared';
import { UserService } from '../../../services';
import { User, UserStatus } from '../../../models';

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
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
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
