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
import { WebhookService } from '../../services';
import { Webhook, WebhookStatus } from '../../models';

@Component({
  selector: 'app-webhook-list',
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
      title="Webhooks"
      subtitle="Manage webhook endpoints"
      icon="webhook"
    >
      <button mat-raised-button color="primary" routerLink="/webhooks/new">
        <mat-icon>add</mat-icon>
        New Webhook
      </button>
    </app-page-header>

    <mat-card class="webhook-list__card">
      <mat-card-content>
        <div class="webhook-list__toolbar">
          <mat-form-field appearance="outline" class="webhook-list__search">
            <mat-label>Search webhooks</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading webhooks..."></app-loading-spinner>
        } @else if (filteredWebhooks().length === 0) {
          <app-empty-state
            icon="webhook"
            title="No webhooks found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by creating your first webhook'"
            [actionLabel]="searchQuery() ? undefined : 'Create Webhook'"
            actionIcon="add"
            (action)="router.navigate(['/webhooks/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredWebhooks()" class="webhook-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let webhook">
                <a [routerLink]="['/webhooks', webhook.webhookId]" class="webhook-list__link">
                  {{ webhook.name }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="url">
              <th mat-header-cell *matHeaderCellDef>URL</th>
              <td mat-cell *matCellDef="let webhook">
                <code class="webhook-list__url">{{ truncateUrl(webhook.url) }}</code>
              </td>
            </ng-container>

            <ng-container matColumnDef="events">
              <th mat-header-cell *matHeaderCellDef>Events</th>
              <td mat-cell *matCellDef="let webhook">{{ webhook.events?.length || 0 }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let webhook">
                <app-status-badge
                  [status]="getStatusType(webhook.status)"
                  [label]="webhook.status"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let webhook">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/webhooks', webhook.webhookId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View</span>
                  </button>
                  <button mat-menu-item [routerLink]="['/webhooks', webhook.webhookId, 'edit']">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  <button mat-menu-item (click)="testWebhook(webhook)">
                    <mat-icon>send</mat-icon>
                    <span>Test</span>
                  </button>
                  @if (webhook.status === 'Active') {
                    <button mat-menu-item (click)="disableWebhook(webhook)">
                      <mat-icon>pause</mat-icon>
                      <span>Disable</span>
                    </button>
                  } @else {
                    <button mat-menu-item (click)="enableWebhook(webhook)">
                      <mat-icon>play_arrow</mat-icon>
                      <span>Enable</span>
                    </button>
                  }
                  <button mat-menu-item (click)="deleteWebhook(webhook)" class="webhook-list__delete">
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
    .webhook-list {
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

      &__url {
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
export class WebhookList implements OnInit {
  readonly router = inject(Router);
  private readonly webhookService = inject(WebhookService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly webhooks = signal<Webhook[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'url', 'events', 'status', 'actions'];

  readonly filteredWebhooks = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.webhooks();
    return this.webhooks().filter(w =>
      w.name.toLowerCase().includes(query) ||
      w.url.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadWebhooks();
  }

  private loadWebhooks(): void {
    this.webhookService.getAll().subscribe({
      next: (webhooks) => {
        this.webhooks.set(webhooks);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStatusType(status: WebhookStatus): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (status) {
      case WebhookStatus.Active: return 'success';
      case WebhookStatus.Inactive: return 'neutral';
      case WebhookStatus.Failed: return 'error';
      default: return 'neutral';
    }
  }

  truncateUrl(url: string): string {
    if (url.length > 40) {
      return url.substring(0, 40) + '...';
    }
    return url;
  }

  testWebhook(webhook: Webhook): void {
    this.webhookService.test(webhook.webhookId).subscribe({
      next: () => {
        this.snackBar.open('Test webhook sent successfully', 'Close', { duration: 3000 });
      },
      error: () => {
        this.snackBar.open('Failed to send test webhook', 'Close', { duration: 3000 });
      }
    });
  }

  enableWebhook(webhook: Webhook): void {
    this.webhookService.enable(webhook.webhookId).subscribe({
      next: () => {
        this.snackBar.open('Webhook enabled successfully', 'Close', { duration: 3000 });
        this.loadWebhooks();
      }
    });
  }

  disableWebhook(webhook: Webhook): void {
    this.webhookService.disable(webhook.webhookId).subscribe({
      next: () => {
        this.snackBar.open('Webhook disabled successfully', 'Close', { duration: 3000 });
        this.loadWebhooks();
      }
    });
  }

  deleteWebhook(webhook: Webhook): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Webhook',
        message: `Are you sure you want to delete "${webhook.name}"?`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.webhookService.delete(webhook.webhookId).subscribe({
          next: () => {
            this.snackBar.open('Webhook deleted successfully', 'Close', { duration: 3000 });
            this.loadWebhooks();
          }
        });
      }
    });
  }
}
