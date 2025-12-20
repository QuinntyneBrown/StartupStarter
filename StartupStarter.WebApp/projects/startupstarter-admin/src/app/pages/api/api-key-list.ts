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
import { ApiKeyService } from '../../services';
import { ApiKey, ApiKeyStatus } from '../../models';

@Component({
  selector: 'app-api-key-list',
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
      title="API Keys"
      subtitle="Manage API keys for integrations"
      icon="key"
    >
      <button mat-raised-button color="primary" routerLink="/api-keys/new">
        <mat-icon>add</mat-icon>
        Generate Key
      </button>
    </app-page-header>

    <mat-card class="api-key-list__card">
      <mat-card-content>
        <div class="api-key-list__toolbar">
          <mat-form-field appearance="outline" class="api-key-list__search">
            <mat-label>Search API keys</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading API keys..."></app-loading-spinner>
        } @else if (filteredApiKeys().length === 0) {
          <app-empty-state
            icon="key"
            title="No API keys found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by generating your first API key'"
            [actionLabel]="searchQuery() ? undefined : 'Generate Key'"
            actionIcon="add"
            (action)="router.navigate(['/api-keys/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredApiKeys()" class="api-key-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let apiKey">
                <a [routerLink]="['/api-keys', apiKey.apiKeyId]" class="api-key-list__link">
                  {{ apiKey.name }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="keyPrefix">
              <th mat-header-cell *matHeaderCellDef>Key</th>
              <td mat-cell *matCellDef="let apiKey">
                <code class="api-key-list__key">{{ apiKey.keyPrefix }}...</code>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let apiKey">
                <app-status-badge
                  [status]="getStatusType(apiKey.status)"
                  [label]="apiKey.status"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="lastUsed">
              <th mat-header-cell *matHeaderCellDef>Last Used</th>
              <td mat-cell *matCellDef="let apiKey">
                {{ apiKey.lastUsedAt ? formatDate(apiKey.lastUsedAt) : 'Never' }}
              </td>
            </ng-container>

            <ng-container matColumnDef="expires">
              <th mat-header-cell *matHeaderCellDef>Expires</th>
              <td mat-cell *matCellDef="let apiKey">
                {{ apiKey.expiresAt ? formatDate(apiKey.expiresAt) : 'Never' }}
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let apiKey">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/api-keys', apiKey.apiKeyId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View</span>
                  </button>
                  @if (apiKey.status === 'Active') {
                    <button mat-menu-item (click)="revokeApiKey(apiKey)">
                      <mat-icon>block</mat-icon>
                      <span>Revoke</span>
                    </button>
                  }
                  <button mat-menu-item (click)="deleteApiKey(apiKey)" class="api-key-list__delete">
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
    .api-key-list {
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

      &__key {
        padding: var(--spacing-xs) var(--spacing-sm);
        background: var(--mat-sys-surface-variant);
        border-radius: var(--spacing-xs);
        font-family: monospace;
        font-size: 12px;
      }

      &__delete {
        color: var(--mat-sys-error);
      }
    }
  `]
})
export class ApiKeyList implements OnInit {
  readonly router = inject(Router);
  private readonly apiKeyService = inject(ApiKeyService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly apiKeys = signal<ApiKey[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'keyPrefix', 'status', 'lastUsed', 'expires', 'actions'];

  readonly filteredApiKeys = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.apiKeys();
    return this.apiKeys().filter(k =>
      k.name.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadApiKeys();
  }

  private loadApiKeys(): void {
    this.apiKeyService.getAll().subscribe({
      next: (apiKeys) => {
        this.apiKeys.set(apiKeys);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStatusType(status: ApiKeyStatus): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (status) {
      case ApiKeyStatus.Active: return 'success';
      case ApiKeyStatus.Expired: return 'warning';
      case ApiKeyStatus.Revoked: return 'error';
      default: return 'neutral';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }

  revokeApiKey(apiKey: ApiKey): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Revoke API Key',
        message: `Are you sure you want to revoke "${apiKey.name}"? This action cannot be undone.`,
        confirmText: 'Revoke',
        confirmColor: 'warn',
        icon: 'block'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.apiKeyService.revoke(apiKey.apiKeyId, 'Revoked by admin').subscribe({
          next: () => {
            this.snackBar.open('API key revoked successfully', 'Close', { duration: 3000 });
            this.loadApiKeys();
          }
        });
      }
    });
  }

  deleteApiKey(apiKey: ApiKey): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete API Key',
        message: `Are you sure you want to delete "${apiKey.name}"?`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.apiKeyService.delete(apiKey.apiKeyId).subscribe({
          next: () => {
            this.snackBar.open('API key deleted successfully', 'Close', { duration: 3000 });
            this.loadApiKeys();
          }
        });
      }
    });
  }
}
