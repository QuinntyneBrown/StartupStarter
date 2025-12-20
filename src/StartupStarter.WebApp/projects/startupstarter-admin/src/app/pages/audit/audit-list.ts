import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { PageHeader } from '../../components/shared/page-header';
import { StatusBadge } from '../../components/shared/status-badge';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { EmptyState } from '../../components/shared/empty-state';
import { AuditService } from '../../services';
import { AuditLog, AuditAction } from '../../models';

@Component({
  selector: 'app-audit-list',
  standalone: true,
  imports: [
    FormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    PageHeader,
    StatusBadge,
    LoadingSpinner,
    EmptyState
  ],
  template: `
    <app-page-header
      title="Audit Log"
      subtitle="View system activity and changes"
      icon="history"
    >
      <button mat-raised-button (click)="exportAuditLog()">
        <mat-icon>download</mat-icon>
        Export
      </button>
    </app-page-header>

    <mat-card class="audit-list__card">
      <mat-card-content>
        <div class="audit-list__toolbar">
          <mat-form-field appearance="outline" class="audit-list__search">
            <mat-label>Search</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by user or entity...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>

          <mat-form-field appearance="outline" class="audit-list__filter">
            <mat-label>Action</mat-label>
            <mat-select [(ngModel)]="selectedAction">
              <mat-option value="">All Actions</mat-option>
              @for (action of actions; track action) {
                <mat-option [value]="action">{{ action }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline" class="audit-list__filter">
            <mat-label>Entity Type</mat-label>
            <mat-select [(ngModel)]="selectedEntityType">
              <mat-option value="">All Types</mat-option>
              @for (type of entityTypes; track type) {
                <mat-option [value]="type">{{ type }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading audit log..."></app-loading-spinner>
        } @else if (filteredLogs().length === 0) {
          <app-empty-state
            icon="history"
            title="No audit logs found"
            message="No activity matching your search criteria"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredLogs()" class="audit-list__table">
            <ng-container matColumnDef="timestamp">
              <th mat-header-cell *matHeaderCellDef>Timestamp</th>
              <td mat-cell *matCellDef="let log">{{ formatDateTime(log.timestamp) }}</td>
            </ng-container>

            <ng-container matColumnDef="user">
              <th mat-header-cell *matHeaderCellDef>User</th>
              <td mat-cell *matCellDef="let log">
                <div class="audit-list__user">
                  <span class="audit-list__avatar">{{ getInitials(log.userId) }}</span>
                  {{ log.userId }}
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="action">
              <th mat-header-cell *matHeaderCellDef>Action</th>
              <td mat-cell *matCellDef="let log">
                <app-status-badge
                  [status]="getActionType(log.action)"
                  [label]="log.action"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="entityType">
              <th mat-header-cell *matHeaderCellDef>Entity</th>
              <td mat-cell *matCellDef="let log">{{ log.entityType }}</td>
            </ng-container>

            <ng-container matColumnDef="entityId">
              <th mat-header-cell *matHeaderCellDef>Entity ID</th>
              <td mat-cell *matCellDef="let log">
                <code class="audit-list__entity-id">{{ log.entityId }}</code>
              </td>
            </ng-container>

            <ng-container matColumnDef="ipAddress">
              <th mat-header-cell *matHeaderCellDef>IP Address</th>
              <td mat-cell *matCellDef="let log">{{ log.ipAddress }}</td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .audit-list {
      &__card {
        overflow: hidden;
      }

      &__toolbar {
        display: flex;
        flex-wrap: wrap;
        gap: var(--spacing-md);
        margin-bottom: var(--spacing-md);
      }

      &__search {
        flex: 1;
        min-width: 200px;
        max-width: 300px;
      }

      &__filter {
        min-width: 150px;
      }

      &__table {
        width: 100%;
      }

      &__user {
        display: flex;
        align-items: center;
        gap: var(--spacing-sm);
      }

      &__avatar {
        width: 28px;
        height: 28px;
        border-radius: 50%;
        background: var(--mat-sys-primary-container);
        color: var(--mat-sys-on-primary-container);
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 11px;
        font-weight: 500;
      }

      &__entity-id {
        padding: var(--spacing-xs) var(--spacing-sm);
        background: var(--mat-sys-surface-variant);
        border-radius: var(--spacing-xs);
        font-family: monospace;
        font-size: 11px;
      }
    }
  `]
})
export class AuditList implements OnInit {
  private readonly auditService = inject(AuditService);

  readonly isLoading = signal(true);
  readonly logs = signal<AuditLog[]>([]);
  readonly searchQuery = signal('');
  readonly selectedAction = signal('');
  readonly selectedEntityType = signal('');

  readonly displayedColumns = ['timestamp', 'user', 'action', 'entityType', 'entityId', 'ipAddress'];

  readonly actions = Object.values(AuditAction);
  readonly entityTypes = ['Account', 'User', 'Role', 'Content', 'Media', 'Profile', 'Workflow', 'ApiKey', 'Webhook', 'System'];

  readonly filteredLogs = computed(() => {
    let result = this.logs();
    const query = this.searchQuery().toLowerCase();
    const action = this.selectedAction();
    const entityType = this.selectedEntityType();

    if (query) {
      result = result.filter(log =>
        log.userId.toLowerCase().includes(query) ||
        log.entityId.toLowerCase().includes(query)
      );
    }

    if (action) {
      result = result.filter(log => log.action === action);
    }

    if (entityType) {
      result = result.filter(log => log.entityType === entityType);
    }

    return result;
  });

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  private loadAuditLogs(): void {
    this.auditService.getAll().subscribe({
      next: (logs) => {
        this.logs.set(logs);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getActionType(action: AuditAction): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (action) {
      case AuditAction.Create: return 'success';
      case AuditAction.Update: return 'info';
      case AuditAction.Delete: return 'error';
      case AuditAction.Login: return 'success';
      case AuditAction.Logout: return 'neutral';
      case AuditAction.LoginFailed: return 'error';
      case AuditAction.PasswordChanged: return 'warning';
      default: return 'neutral';
    }
  }

  formatDateTime(date: Date): string {
    return new Date(date).toLocaleString();
  }

  getInitials(userId: string): string {
    return userId.substring(0, 2).toUpperCase();
  }

  exportAuditLog(): void {
    this.auditService.export({}).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `audit-log-${new Date().toISOString().split('T')[0]}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }
}
