import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent } from '../../../components/shared';
import { WebhookService } from '../../../services';
import { Webhook } from '../../../models';

@Component({
  selector: 'app-webhook-list',
  standalone: true,
  imports: [
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule, MatChipsModule,
    PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  templateUrl: './webhook-list.component.html',
  styleUrl: './webhook-list.component.scss'
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
