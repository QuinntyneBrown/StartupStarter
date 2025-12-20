import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  SystemMaintenance,
  SystemBackup,
  SystemError,
  SystemHealth,
  SystemMetrics,
  ScheduleMaintenanceRequest,
  ResolveErrorRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class SystemService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'system';

  // Maintenance
  getMaintenances(): Observable<SystemMaintenance[]> {
    return this.api.get<SystemMaintenance[]>(`${this.endpoint}/maintenance`);
  }

  getMaintenance(id: string): Observable<SystemMaintenance> {
    return this.api.get<SystemMaintenance>(`${this.endpoint}/maintenance/${id}`);
  }

  scheduleMaintenance(request: ScheduleMaintenanceRequest): Observable<SystemMaintenance> {
    return this.api.post<SystemMaintenance>(`${this.endpoint}/maintenance`, request);
  }

  startMaintenance(id: string): Observable<SystemMaintenance> {
    return this.api.put<SystemMaintenance>(`${this.endpoint}/maintenance/${id}/start`, {});
  }

  completeMaintenance(id: string): Observable<SystemMaintenance> {
    return this.api.put<SystemMaintenance>(`${this.endpoint}/maintenance/${id}/complete`, {});
  }

  cancelMaintenance(id: string): Observable<SystemMaintenance> {
    return this.api.put<SystemMaintenance>(`${this.endpoint}/maintenance/${id}/cancel`, {});
  }

  // Backups
  getBackups(): Observable<SystemBackup[]> {
    return this.api.get<SystemBackup[]>(`${this.endpoint}/backups`);
  }

  getBackup(id: string): Observable<SystemBackup> {
    return this.api.get<SystemBackup>(`${this.endpoint}/backups/${id}`);
  }

  triggerBackup(): Observable<SystemBackup> {
    return this.api.post<SystemBackup>(`${this.endpoint}/backup`, {});
  }

  restoreBackup(id: string): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/backups/${id}/restore`, {});
  }

  // Errors
  getErrors(): Observable<SystemError[]> {
    return this.api.get<SystemError[]>(`${this.endpoint}/errors`);
  }

  getError(id: string): Observable<SystemError> {
    return this.api.get<SystemError>(`${this.endpoint}/errors/${id}`);
  }

  resolveError(id: string, request: ResolveErrorRequest): Observable<SystemError> {
    return this.api.put<SystemError>(`${this.endpoint}/errors/${id}/resolve`, request);
  }

  // Health & Metrics
  getHealth(): Observable<SystemHealth> {
    return this.api.get<SystemHealth>(`${this.endpoint}/health`);
  }

  getMetrics(): Observable<SystemMetrics> {
    return this.api.get<SystemMetrics>(`${this.endpoint}/metrics`);
  }
}
