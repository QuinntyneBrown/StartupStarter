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
  templateUrl: './system.component.html',
  styleUrl: './system.component.scss'
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
