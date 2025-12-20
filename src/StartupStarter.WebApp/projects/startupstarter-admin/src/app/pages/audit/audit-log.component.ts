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
  templateUrl: './audit-log.component.html',
  styleUrl: './audit-log.component.scss'
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
