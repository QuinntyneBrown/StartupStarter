import { Component, inject, signal, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent } from '../../components/shared';
import { AuditService } from '../../services';
import { AuditLog, AuditAction, ExportFormat } from '../../models';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [
    FormsModule, DatePipe, MatCardModule, MatTableModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule,
    PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="Audit Logs" subtitle="View system activity and changes" icon="history">
      <button mat-stroked-button (click)="exportLogs()">
        <mat-icon>download</mat-icon>
        Export
      </button>
    </app-page-header>

    <mat-card class="filters-card">
      <mat-card-content>
        <div class="filters">
          <mat-form-field appearance="outline">
            <mat-label>Entity Type</mat-label>
            <mat-select [(ngModel)]="filters.entityType" (selectionChange)="load()">
              <mat-option value="">All</mat-option>
              <mat-option value="Account">Account</mat-option>
              <mat-option value="User">User</mat-option>
              <mat-option value="Role">Role</mat-option>
              <mat-option value="Content">Content</mat-option>
              <mat-option value="Media">Media</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Action</mat-label>
            <mat-select [(ngModel)]="filters.action" (selectionChange)="load()">
              <mat-option value="">All</mat-option>
              @for (action of actions; track action) {
                <mat-option [value]="action">{{ action }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Start Date</mat-label>
            <input matInput [matDatepicker]="startPicker" [(ngModel)]="filters.startDate" (dateChange)="load()">
            <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
            <mat-datepicker #startPicker></mat-datepicker>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>End Date</mat-label>
            <input matInput [matDatepicker]="endPicker" [(ngModel)]="filters.endDate" (dateChange)="load()">
            <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
            <mat-datepicker #endPicker></mat-datepicker>
          </mat-form-field>
        </div>
      </mat-card-content>
    </mat-card>

    <mat-card>
      <mat-card-content>
        @if (isLoading()) {
          <app-loading-spinner message="Loading audit logs..."></app-loading-spinner>
        } @else if (logs().length === 0) {
          <app-empty-state icon="history" title="No audit logs found" message="Try adjusting your filters"></app-empty-state>
        } @else {
          <table mat-table [dataSource]="logs()" class="full-width">
            <ng-container matColumnDef="timestamp">
              <th mat-header-cell *matHeaderCellDef>Timestamp</th>
              <td mat-cell *matCellDef="let log">{{ log.timestamp | date:'short' }}</td>
            </ng-container>
            <ng-container matColumnDef="action">
              <th mat-header-cell *matHeaderCellDef>Action</th>
              <td mat-cell *matCellDef="let log">
                <app-status-badge [label]="log.action" [type]="getActionType(log.action)"></app-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="entityType">
              <th mat-header-cell *matHeaderCellDef>Entity</th>
              <td mat-cell *matCellDef="let log">{{ log.entityType }}</td>
            </ng-container>
            <ng-container matColumnDef="user">
              <th mat-header-cell *matHeaderCellDef>User</th>
              <td mat-cell *matCellDef="let log">{{ log.userName || log.userId }}</td>
            </ng-container>
            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let log">{{ log.description || '-' }}</td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .filters-card { margin-bottom: 16px; }
    .filters { display: flex; gap: 16px; flex-wrap: wrap; }
    .filters mat-form-field { min-width: 150px; }
    .full-width { width: 100%; }
  `]
})
export class AuditLogComponent implements OnInit {
  private readonly auditService = inject(AuditService);

  readonly isLoading = signal(true);
  readonly logs = signal<AuditLog[]>([]);
  actions = Object.values(AuditAction);
  displayedColumns = ['timestamp', 'action', 'entityType', 'user', 'description'];

  filters: { entityType?: string; action?: string; startDate?: Date; endDate?: Date } = {};

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.auditService.getLogs(this.filters as any).subscribe({
      next: (logs) => { this.logs.set(logs); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getActionType(action: AuditAction): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (action) {
      case AuditAction.Create: return 'success';
      case AuditAction.Delete: return 'error';
      case AuditAction.Update: return 'info';
      case AuditAction.Lock: case AuditAction.Deactivate: return 'warning';
      default: return 'neutral';
    }
  }

  exportLogs(): void {
    this.auditService.requestExport({ format: ExportFormat.CSV, filters: this.filters as any }).subscribe();
  }
}
