import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent } from '../../../components/shared';
import { ApiKeyService } from '../../../services';
import { ApiKey } from '../../../models';
import { ApiKeyFormDialogComponent } from '../api-key-form-dialog/api-key-form-dialog.component';

@Component({
  selector: 'app-api-key-list',
  standalone: true,
  imports: [
    DatePipe, MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  templateUrl: './api-key-list.component.html',
  styleUrl: './api-key-list.component.scss'
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
