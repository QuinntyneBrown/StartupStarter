import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent } from '../../components/shared';
import { ApiKeyService } from '../../services';
import { ApiKey } from '../../models';
import { ApiKeyFormDialogComponent } from './api-key-form-dialog.component';

@Component({
  selector: 'app-api-key-list',
  standalone: true,
  imports: [
    DatePipe, MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="API Keys" subtitle="Manage API keys for external integrations" icon="vpn_key">
      <button mat-flat-button color="primary" (click)="create()">
        <mat-icon>add</mat-icon>
        New API Key
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        @if (isLoading()) {
          <app-loading-spinner message="Loading API keys..."></app-loading-spinner>
        } @else if (apiKeys().length === 0) {
          <app-empty-state icon="vpn_key" title="No API keys" message="Create an API key for external integrations" actionLabel="Create API Key" (action)="create()"></app-empty-state>
        } @else {
          <table mat-table [dataSource]="apiKeys()" class="full-width">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let key">{{ key.keyName }}</td>
            </ng-container>
            <ng-container matColumnDef="prefix">
              <th mat-header-cell *matHeaderCellDef>Key Prefix</th>
              <td mat-cell *matCellDef="let key">{{ key.keyPrefix }}...</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let key">
                <app-status-badge [label]="key.isActive ? 'Active' : 'Revoked'" [type]="key.isActive ? 'success' : 'error'"></app-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="lastUsed">
              <th mat-header-cell *matHeaderCellDef>Last Used</th>
              <td mat-cell *matCellDef="let key">{{ key.lastUsedAt ? (key.lastUsedAt | date:'short') : 'Never' }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let key">
                <button mat-icon-button color="warn" (click)="revoke(key)" [disabled]="!key.isActive">
                  <mat-icon>delete</mat-icon>
                </button>
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
export class ApiKeyListComponent implements OnInit {
  private readonly apiKeyService = inject(ApiKeyService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly apiKeys = signal<ApiKey[]>([]);
  displayedColumns = ['name', 'prefix', 'status', 'lastUsed', 'actions'];

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.apiKeyService.getAll().subscribe({
      next: (keys) => { this.apiKeys.set(keys); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  create(): void {
    const dialogRef = this.dialog.open(ApiKeyFormDialogComponent, { width: '500px' });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open(`API Key created: ${result.apiKey}`, 'Copy', { duration: 10000 });
        this.load();
      }
    });
  }

  revoke(key: ApiKey): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Revoke API Key', message: `Revoke "${key.keyName}"? This cannot be undone.`, confirmText: 'Revoke', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) this.apiKeyService.revoke(key.apiKeyId).subscribe(() => this.load());
    });
  }
}
