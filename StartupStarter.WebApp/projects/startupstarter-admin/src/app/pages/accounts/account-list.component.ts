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
import { AccountService } from '../../services';
import { Account, AccountStatus } from '../../models';

@Component({
  selector: 'app-account-list',
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
    <app-page-header title="Accounts" subtitle="Manage tenant accounts" icon="business">
      <button mat-flat-button color="primary" (click)="create()">
        <mat-icon>add</mat-icon>
        New Account
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search accounts</mat-label>
          <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        @if (isLoading()) {
          <app-loading-spinner message="Loading accounts..."></app-loading-spinner>
        } @else if (filtered().length === 0) {
          <app-empty-state
            icon="business"
            title="No accounts found"
            [message]="searchQuery ? 'Try adjusting your search' : 'Create your first account to get started'"
            [actionLabel]="searchQuery ? '' : 'Create Account'"
            (action)="create()">
          </app-empty-state>
        } @else {
          <table mat-table [dataSource]="filtered()" class="full-width">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let account">{{ account.accountName }}</td>
            </ng-container>

            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let account">{{ account.accountType }}</td>
            </ng-container>

            <ng-container matColumnDef="subscription">
              <th mat-header-cell *matHeaderCellDef>Subscription</th>
              <td mat-cell *matCellDef="let account">{{ account.subscriptionTier }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let account">
                <app-status-badge [label]="account.status" [type]="getStatusType(account.status)"></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let account">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="edit(account)">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  @if (account.status === 'Active') {
                    <button mat-menu-item (click)="suspend(account)">
                      <mat-icon>pause</mat-icon>
                      <span>Suspend</span>
                    </button>
                  } @else if (account.status === 'Suspended') {
                    <button mat-menu-item (click)="reactivate(account)">
                      <mat-icon>play_arrow</mat-icon>
                      <span>Reactivate</span>
                    </button>
                  }
                  <button mat-menu-item (click)="delete(account)">
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
    .search-field {
      width: 100%;
      max-width: 400px;
      margin-bottom: 16px;
    }

    .full-width {
      width: 100%;
    }
  `]
})
export class AccountListComponent implements OnInit {
  private readonly accountService = inject(AccountService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly accounts = signal<Account[]>([]);
  searchQuery = '';

  displayedColumns = ['name', 'type', 'subscription', 'status', 'actions'];

  readonly filtered = computed(() => {
    const query = this.searchQuery.toLowerCase();
    return this.accounts().filter(a =>
      a.accountName.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
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

  getStatusType(status: AccountStatus): BadgeType {
    switch (status) {
      case AccountStatus.Active: return 'success';
      case AccountStatus.Suspended: return 'warning';
      case AccountStatus.Deleted: return 'error';
      default: return 'neutral';
    }
  }

  create(): void {
    this.router.navigate(['/accounts/new']);
  }

  edit(account: Account): void {
    this.router.navigate(['/accounts', account.accountId]);
  }

  suspend(account: Account): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Suspend Account',
        message: `Are you sure you want to suspend "${account.accountName}"?`,
        confirmText: 'Suspend',
        confirmColor: 'warn'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.accountService.suspend(account.accountId, { reason: 'Manual suspension' }).subscribe(() => {
          this.load();
        });
      }
    });
  }

  reactivate(account: Account): void {
    this.accountService.reactivate(account.accountId).subscribe(() => {
      this.load();
    });
  }

  delete(account: Account): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Account',
        message: `Are you sure you want to delete "${account.accountName}"? This action cannot be undone.`,
        confirmText: 'Delete',
        confirmColor: 'warn'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.accountService.delete(account.accountId).subscribe(() => {
          this.load();
        });
      }
    });
  }
}
