import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  SystemMaintenance,
  SystemBackup,
  SystemError,
  SystemConfig,
  ScheduleMaintenanceRequest,
  StartBackupRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class SystemService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'system';

  // Maintenance operations
  getMaintenanceSchedule(): Observable<SystemMaintenance[]> {
    return this.api.get<SystemMaintenance[]>(`${this.endpoint}/maintenance`);
  }

  getMaintenanceById(maintenanceId: string): Observable<SystemMaintenance> {
    return this.api.get<SystemMaintenance>(`${this.endpoint}/maintenance/${maintenanceId}`);
  }

  scheduleMaintenance(request: ScheduleMaintenanceRequest): Observable<SystemMaintenance> {
    return this.api.post<SystemMaintenance>(`${this.endpoint}/maintenance`, request);
  }

  startMaintenance(maintenanceId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/maintenance/${maintenanceId}/start`, {});
  }

  completeMaintenance(maintenanceId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/maintenance/${maintenanceId}/complete`, {});
  }

  cancelMaintenance(maintenanceId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/maintenance/${maintenanceId}`);
  }

  // Backup operations
  getBackups(): Observable<SystemBackup[]> {
    return this.api.get<SystemBackup[]>(`${this.endpoint}/backups`);
  }

  getBackupById(backupId: string): Observable<SystemBackup> {
    return this.api.get<SystemBackup>(`${this.endpoint}/backups/${backupId}`);
  }

  startBackup(request: StartBackupRequest): Observable<SystemBackup> {
    return this.api.post<SystemBackup>(`${this.endpoint}/backups`, request);
  }

  restoreBackup(backupId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/backups/${backupId}/restore`, {});
  }

  deleteBackup(backupId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/backups/${backupId}`);
  }

  // Error tracking
  getErrors(severity?: string, startDate?: Date, endDate?: Date): Observable<SystemError[]> {
    const params: Record<string, string> = {};
    if (severity) params['severity'] = severity;
    if (startDate) params['startDate'] = startDate.toISOString();
    if (endDate) params['endDate'] = endDate.toISOString();
    return this.api.get<SystemError[]>(`${this.endpoint}/errors`, params);
  }

  getErrorById(errorId: string): Observable<SystemError> {
    return this.api.get<SystemError>(`${this.endpoint}/errors/${errorId}`);
  }

  resolveError(errorId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/errors/${errorId}/resolve`, {});
  }

  // System health
  getHealth(): Observable<{ status: string; services: Record<string, string> }> {
    return this.api.get<{ status: string; services: Record<string, string> }>(`${this.endpoint}/health`);
  }

  getMetrics(): Observable<Record<string, number>> {
    return this.api.get<Record<string, number>>(`${this.endpoint}/metrics`);
  }

  // Settings operations
  getSettings(): Observable<SystemConfig> {
    return this.api.get<SystemConfig>(`${this.endpoint}/settings`);
  }

  updateSettings(settings: Partial<SystemConfig>): Observable<SystemConfig> {
    return this.api.put<SystemConfig>(`${this.endpoint}/settings`, settings);
  }

  setMaintenanceMode(enabled: boolean): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/maintenance-mode`, { enabled });
  }

  clearCache(): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/cache/clear`, {});
  }
}
