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
import { AccountService } from '../../services';
import { Account, AccountStatus } from '../../models';

@Component({
  selector: 'app-account-list',
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
      title="Accounts"
      subtitle="Manage organization accounts"
      icon="business"
    >
      <button mat-raised-button color="primary" routerLink="/accounts/new">
        <mat-icon>add</mat-icon>
        New Account
      </button>
    </app-page-header>

    <mat-card class="account-list__card">
      <mat-card-content>
        <div class="account-list__toolbar">
          <mat-form-field appearance="outline" class="account-list__search">
            <mat-label>Search accounts</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading accounts..."></app-loading-spinner>
        } @else if (filteredAccounts().length === 0) {
          <app-empty-state
            icon="business"
            title="No accounts found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by creating your first account'"
            [actionLabel]="searchQuery() ? undefined : 'Create Account'"
            actionIcon="add"
            (action)="router.navigate(['/accounts/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredAccounts()" class="account-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let account">
                <a [routerLink]="['/accounts', account.accountId]" class="account-list__link">
                  {{ account.accountName }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let account">{{ account.accountType }}</td>
            </ng-container>

            <ng-container matColumnDef="tier">
              <th mat-header-cell *matHeaderCellDef>Subscription</th>
              <td mat-cell *matCellDef="let account">{{ account.subscriptionTier }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let account">
                <app-status-badge
                  [status]="getStatusType(account.status)"
                  [label]="account.status"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="created">
              <th mat-header-cell *matHeaderCellDef>Created</th>
              <td mat-cell *matCellDef="let account">{{ formatDate(account.createdAt) }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let account">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/accounts', account.accountId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View Details</span>
                  </button>
                  <button mat-menu-item [routerLink]="['/accounts', account.accountId, 'edit']">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  @if (account.status === 'Active') {
                    <button mat-menu-item (click)="suspendAccount(account)">
                      <mat-icon>pause_circle</mat-icon>
                      <span>Suspend</span>
                    </button>
                  } @else if (account.status === 'Suspended') {
                    <button mat-menu-item (click)="reactivateAccount(account)">
                      <mat-icon>play_circle</mat-icon>
                      <span>Reactivate</span>
                    </button>
                  }
                  <button mat-menu-item (click)="deleteAccount(account)" class="account-list__delete">
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
    .account-list {
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
export class AccountList implements OnInit {
  readonly router = inject(Router);
  private readonly accountService = inject(AccountService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly accounts = signal<Account[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'type', 'tier', 'status', 'created', 'actions'];

  readonly filteredAccounts = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.accounts();
    return this.accounts().filter(a =>
      a.accountName.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadAccounts();
  }

  private loadAccounts(): void {
    this.accountService.getAll().subscribe({
      next: (accounts) => {
        this.accounts.set(accounts);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStatusType(status: AccountStatus): 'success' | 'warning' | 'error' | 'neutral' {
    switch (status) {
      case AccountStatus.Active: return 'success';
      case AccountStatus.Suspended: return 'warning';
      case AccountStatus.Deleted: return 'error';
      default: return 'neutral';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }

  suspendAccount(account: Account): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Suspend Account',
        message: `Are you sure you want to suspend "${account.accountName}"? Users will not be able to access this account.`,
        confirmText: 'Suspend',
        confirmColor: 'warn',
        icon: 'pause_circle'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.accountService.suspend(account.accountId, 'Suspended by admin').subscribe({
          next: () => {
            this.snackBar.open('Account suspended successfully', 'Close', { duration: 3000 });
            this.loadAccounts();
          }
        });
      }
    });
  }

  reactivateAccount(account: Account): void {
    this.accountService.reactivate(account.accountId).subscribe({
      next: () => {
        this.snackBar.open('Account reactivated successfully', 'Close', { duration: 3000 });
        this.loadAccounts();
      }
    });
  }

  deleteAccount(account: Account): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Account',
        message: `Are you sure you want to delete "${account.accountName}"? This action cannot be undone.`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.accountService.delete(account.accountId).subscribe({
          next: () => {
            this.snackBar.open('Account deleted successfully', 'Close', { duration: 3000 });
            this.loadAccounts();
          }
        });
      }
    });
  }
}
