import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent } from '../../components/shared';
import { WebhookService } from '../../services';
import { Webhook } from '../../models';

@Component({
  selector: 'app-webhook-list',
  standalone: true,
  imports: [
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule, MatChipsModule,
    PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="Webhooks" subtitle="Manage webhook integrations" icon="webhook">
      <button mat-flat-button color="primary" (click)="create()">
        <mat-icon>add</mat-icon>
        New Webhook
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        @if (isLoading()) {
          <app-loading-spinner message="Loading webhooks..."></app-loading-spinner>
        } @else if (webhooks().length === 0) {
          <app-empty-state icon="webhook" title="No webhooks" message="Create a webhook to receive event notifications" actionLabel="Create Webhook" (action)="create()"></app-empty-state>
        } @else {
          <table mat-table [dataSource]="webhooks()" class="full-width">
            <ng-container matColumnDef="url">
              <th mat-header-cell *matHeaderCellDef>URL</th>
              <td mat-cell *matCellDef="let webhook">{{ webhook.url }}</td>
            </ng-container>
            <ng-container matColumnDef="events">
              <th mat-header-cell *matHeaderCellDef>Events</th>
              <td mat-cell *matCellDef="let webhook">
                <mat-chip-set>
                  @for (event of webhook.events.slice(0, 2); track event) {
                    <mat-chip>{{ event }}</mat-chip>
                  }
                  @if (webhook.events.length > 2) {
                    <mat-chip>+{{ webhook.events.length - 2 }}</mat-chip>
                  }
                </mat-chip-set>
              </td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let webhook">
                <app-status-badge [label]="webhook.isActive ? 'Active' : 'Disabled'" [type]="webhook.isActive ? 'success' : 'neutral'"></app-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="failures">
              <th mat-header-cell *matHeaderCellDef>Failures</th>
              <td mat-cell *matCellDef="let webhook">
                <app-status-badge [label]="webhook.failureCount.toString()" [type]="webhook.failureCount > 5 ? 'error' : webhook.failureCount > 0 ? 'warning' : 'success'"></app-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let webhook">
                <button mat-icon-button [matMenuTriggerFor]="menu"><mat-icon>more_vert</mat-icon></button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="edit(webhook)"><mat-icon>edit</mat-icon><span>Edit</span></button>
                  <button mat-menu-item (click)="test(webhook)"><mat-icon>send</mat-icon><span>Test</span></button>
                  @if (webhook.isActive) {
                    <button mat-menu-item (click)="disable(webhook)"><mat-icon>pause</mat-icon><span>Disable</span></button>
                  } @else {
                    <button mat-menu-item (click)="enable(webhook)"><mat-icon>play_arrow</mat-icon><span>Enable</span></button>
                  }
                  <button mat-menu-item (click)="delete(webhook)"><mat-icon color="warn">delete</mat-icon><span>Delete</span></button>
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
  styles: [`.full-width { width: 100%; }`]
})
export class WebhookListComponent implements OnInit {
  private readonly webhookService = inject(WebhookService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly webhooks = signal<Webhook[]>([]);
  displayedColumns = ['url', 'events', 'status', 'failures', 'actions'];

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.webhookService.getAll().subscribe({
      next: (webhooks) => { this.webhooks.set(webhooks); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  create(): void { this.router.navigate(['/webhooks/new']); }
  edit(webhook: Webhook): void { this.router.navigate(['/webhooks', webhook.webhookId]); }
  test(webhook: Webhook): void { this.webhookService.test(webhook.webhookId).subscribe(); }
  enable(webhook: Webhook): void { this.webhookService.enable(webhook.webhookId).subscribe(() => this.load()); }
  disable(webhook: Webhook): void { this.webhookService.disable(webhook.webhookId).subscribe(() => this.load()); }

  delete(webhook: Webhook): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete Webhook', message: 'Delete this webhook?', confirmText: 'Delete', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) this.webhookService.delete(webhook.webhookId).subscribe(() => this.load());
    });
  }
}
