import { Component, inject, signal, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DatePipe, DecimalPipe, PercentPipe } from '@angular/common';
import { PageHeaderComponent, LoadingSpinnerComponent, StatusBadgeComponent, BadgeType } from '../../components/shared';
import { SystemService } from '../../services';
import { SystemHealth, SystemMetrics, SystemMaintenance, SystemBackup, SystemError, MaintenanceStatus, BackupStatus, ErrorSeverity } from '../../models';

@Component({
  selector: 'app-system',
  standalone: true,
  imports: [
    DatePipe, DecimalPipe, PercentPipe, MatCardModule, MatTabsModule, MatTableModule, MatButtonModule, MatIconModule, MatProgressBarModule,
    PageHeaderComponent, LoadingSpinnerComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="System" subtitle="Monitor and manage system health" icon="settings_applications">
      <button mat-flat-button color="primary" (click)="triggerBackup()">
        <mat-icon>backup</mat-icon>
        Backup Now
      </button>
    </app-page-header>

    @if (isLoading()) {
      <app-loading-spinner message="Loading system info..."></app-loading-spinner>
    } @else {
      <div class="system__metrics-grid">
        <mat-card>
          <mat-card-header><mat-card-title>System Status</mat-card-title></mat-card-header>
          <mat-card-content>
            <div class="system__status-indicator" [class]="health()?.status?.toLowerCase()">
              <mat-icon>{{ health()?.status === 'Healthy' ? 'check_circle' : health()?.status === 'Degraded' ? 'warning' : 'error' }}</mat-icon>
              <span>{{ health()?.status }}</span>
            </div>
            <p class="system__uptime">Uptime: {{ formatUptime(health()?.uptime || 0) }}</p>
          </mat-card-content>
        </mat-card>

        <mat-card>
          <mat-card-header><mat-card-title>Performance</mat-card-title></mat-card-header>
          <mat-card-content>
            <div class="system__metric"><span>Requests/sec</span><strong>{{ metrics()?.requestsPerSecond | number:'1.0-0' }}</strong></div>
            <div class="system__metric"><span>Response Time (p95)</span><strong>{{ metrics()?.responseTimeP95 }}ms</strong></div>
            <div class="system__metric"><span>Error Rate</span><strong>{{ metrics()?.errorRate | percent:'1.2-2' }}</strong></div>
          </mat-card-content>
        </mat-card>

        <mat-card>
          <mat-card-header><mat-card-title>Resources</mat-card-title></mat-card-header>
          <mat-card-content>
            <div class="system__resource">
              <span>CPU</span>
              <mat-progress-bar mode="determinate" [value]="(metrics()?.cpuUsage || 0) * 100"></mat-progress-bar>
              <span>{{ (metrics()?.cpuUsage || 0) * 100 | number:'1.0-0' }}%</span>
            </div>
            <div class="system__resource">
              <span>Memory</span>
              <mat-progress-bar mode="determinate" [value]="(metrics()?.memoryUsage || 0) * 100"></mat-progress-bar>
              <span>{{ (metrics()?.memoryUsage || 0) * 100 | number:'1.0-0' }}%</span>
            </div>
            <div class="system__resource">
              <span>Disk</span>
              <mat-progress-bar mode="determinate" [value]="(metrics()?.diskUsage || 0) * 100"></mat-progress-bar>
              <span>{{ (metrics()?.diskUsage || 0) * 100 | number:'1.0-0' }}%</span>
            </div>
          </mat-card-content>
        </mat-card>
      </div>

      <mat-tab-group>
        <mat-tab label="Maintenance">
          <div class="system__tab-content">
            <table mat-table [dataSource]="maintenances()" class="system__table">
              <ng-container matColumnDef="description"><th mat-header-cell *matHeaderCellDef>Description</th><td mat-cell *matCellDef="let m">{{ m.description }}</td></ng-container>
              <ng-container matColumnDef="scheduled"><th mat-header-cell *matHeaderCellDef>Scheduled</th><td mat-cell *matCellDef="let m">{{ m.scheduledStartTime | date:'short' }}</td></ng-container>
              <ng-container matColumnDef="status"><th mat-header-cell *matHeaderCellDef>Status</th><td mat-cell *matCellDef="let m"><app-status-badge [label]="m.status" [type]="getMaintenanceStatusType(m.status)"></app-status-badge></td></ng-container>
              <tr mat-header-row *matHeaderRowDef="['description', 'scheduled', 'status']"></tr>
              <tr mat-row *matRowDef="let row; columns: ['description', 'scheduled', 'status'];"></tr>
            </table>
          </div>
        </mat-tab>
        <mat-tab label="Backups">
          <div class="system__tab-content">
            <table mat-table [dataSource]="backups()" class="system__table">
              <ng-container matColumnDef="type"><th mat-header-cell *matHeaderCellDef>Type</th><td mat-cell *matCellDef="let b">{{ b.backupType }}</td></ng-container>
              <ng-container matColumnDef="started"><th mat-header-cell *matHeaderCellDef>Started</th><td mat-cell *matCellDef="let b">{{ b.startedAt | date:'short' }}</td></ng-container>
              <ng-container matColumnDef="size"><th mat-header-cell *matHeaderCellDef>Size</th><td mat-cell *matCellDef="let b">{{ formatSize(b.backupSize) }}</td></ng-container>
              <ng-container matColumnDef="status"><th mat-header-cell *matHeaderCellDef>Status</th><td mat-cell *matCellDef="let b"><app-status-badge [label]="b.status" [type]="getBackupStatusType(b.status)"></app-status-badge></td></ng-container>
              <tr mat-header-row *matHeaderRowDef="['type', 'started', 'size', 'status']"></tr>
              <tr mat-row *matRowDef="let row; columns: ['type', 'started', 'size', 'status'];"></tr>
            </table>
          </div>
        </mat-tab>
        <mat-tab label="Errors">
          <div class="system__tab-content">
            <table mat-table [dataSource]="errors()" class="system__table">
              <ng-container matColumnDef="severity"><th mat-header-cell *matHeaderCellDef>Severity</th><td mat-cell *matCellDef="let e"><app-status-badge [label]="e.severity" [type]="getSeverityType(e.severity)"></app-status-badge></td></ng-container>
              <ng-container matColumnDef="type"><th mat-header-cell *matHeaderCellDef>Type</th><td mat-cell *matCellDef="let e">{{ e.errorType }}</td></ng-container>
              <ng-container matColumnDef="message"><th mat-header-cell *matHeaderCellDef>Message</th><td mat-cell *matCellDef="let e">{{ e.errorMessage }}</td></ng-container>
              <ng-container matColumnDef="occurred"><th mat-header-cell *matHeaderCellDef>Occurred</th><td mat-cell *matCellDef="let e">{{ e.occurredAt | date:'short' }}</td></ng-container>
              <tr mat-header-row *matHeaderRowDef="['severity', 'type', 'message', 'occurred']"></tr>
              <tr mat-row *matRowDef="let row; columns: ['severity', 'type', 'message', 'occurred'];"></tr>
            </table>
          </div>
        </mat-tab>
      </mat-tab-group>
    }
  `,
  styles: [`
    .system__metrics-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 16px; margin-bottom: 24px; }
    .system__status-indicator { display: flex; align-items: center; gap: 8px; font-size: 24px; font-weight: 500; }
    .system__status-indicator.healthy { color: #4caf50; }
    .system__status-indicator.degraded { color: #ff9800; }
    .system__status-indicator.unhealthy { color: #f44336; }
    .system__uptime { color: rgba(0,0,0,0.6); margin-top: 8px; }
    .system__metric { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid rgba(0,0,0,0.1); }
    .system__resource { display: grid; grid-template-columns: 80px 1fr 50px; align-items: center; gap: 8px; padding: 8px 0; }
    .system__tab-content { padding: 16px 0; }
    .system__table { width: 100%; }
  `]
})
export class SystemComponent implements OnInit {
  private readonly systemService = inject(SystemService);

  readonly isLoading = signal(true);
  readonly health = signal<SystemHealth | null>(null);
  readonly metrics = signal<SystemMetrics | null>(null);
  readonly maintenances = signal<SystemMaintenance[]>([]);
  readonly backups = signal<SystemBackup[]>([]);
  readonly errors = signal<SystemError[]>([]);

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.systemService.getHealth().subscribe({ next: (h) => this.health.set(h) });
    this.systemService.getMetrics().subscribe({ next: (m) => this.metrics.set(m) });
    this.systemService.getMaintenances().subscribe({ next: (m) => this.maintenances.set(m) });
    this.systemService.getBackups().subscribe({ next: (b) => this.backups.set(b) });
    this.systemService.getErrors().subscribe({
      next: (e) => { this.errors.set(e); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  formatUptime(seconds: number): string {
    const days = Math.floor(seconds / 86400);
    const hours = Math.floor((seconds % 86400) / 3600);
    return `${days}d ${hours}h`;
  }

  formatSize(bytes?: number): string {
    if (!bytes) return '-';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
    return (bytes / (1024 * 1024 * 1024)).toFixed(1) + ' GB';
  }

  getMaintenanceStatusType(status: MaintenanceStatus): BadgeType {
    switch (status) { case MaintenanceStatus.Completed: return 'success'; case MaintenanceStatus.InProgress: return 'info'; case MaintenanceStatus.Scheduled: return 'warning'; default: return 'neutral'; }
  }

  getBackupStatusType(status: BackupStatus): BadgeType {
    switch (status) { case BackupStatus.Completed: return 'success'; case BackupStatus.InProgress: return 'info'; case BackupStatus.Failed: return 'error'; default: return 'neutral'; }
  }

  getSeverityType(severity: ErrorSeverity): BadgeType {
    switch (severity) { case ErrorSeverity.Critical: return 'error'; case ErrorSeverity.High: return 'error'; case ErrorSeverity.Medium: return 'warning'; default: return 'neutral'; }
  }

  triggerBackup(): void { this.systemService.triggerBackup().subscribe(() => this.load()); }
}
