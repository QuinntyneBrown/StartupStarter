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
import { AccountService } from '../../../services';
import { Account, AccountStatus } from '../../../models';

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
  templateUrl: './account-list.component.html',
  styleUrl: './account-list.component.scss'
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
