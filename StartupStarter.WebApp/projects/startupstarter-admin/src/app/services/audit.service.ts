import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  AuditLog,
  AuditExport,
  RetentionPolicy,
  RequestAuditExportRequest,
  AuditLogFilter
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuditService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'audit';

  getAll(): Observable<AuditLog[]> {
    return this.api.get<AuditLog[]>(`${this.endpoint}/logs`);
  }

  getLogs(filter?: AuditLogFilter): Observable<AuditLog[]> {
    const params: Record<string, string> = {};
    if (filter) {
      if (filter.accountId) params['accountId'] = filter.accountId;
      if (filter.entityType) params['entityType'] = filter.entityType;
      if (filter.entityId) params['entityId'] = filter.entityId;
      if (filter.action) params['action'] = filter.action;
      if (filter.startDate) params['startDate'] = filter.startDate.toISOString();
      if (filter.endDate) params['endDate'] = filter.endDate.toISOString();
    }
    return this.api.get<AuditLog[]>(`${this.endpoint}/logs`, params);
  }

  getLogById(auditId: string): Observable<AuditLog> {
    return this.api.get<AuditLog>(`${this.endpoint}/logs/${auditId}`);
  }

  getLogsByEntity(entityType: string, entityId: string): Observable<AuditLog[]> {
    return this.api.get<AuditLog[]>(`${this.endpoint}/logs/entity/${entityType}/${entityId}`);
  }

  getLogsByAccount(accountId: string, startDate?: Date, endDate?: Date): Observable<AuditLog[]> {
    const params: Record<string, string> = {};
    if (startDate) params['startDate'] = startDate.toISOString();
    if (endDate) params['endDate'] = endDate.toISOString();
    return this.api.get<AuditLog[]>(`${this.endpoint}/logs/account/${accountId}`, params);
  }

  // Export operations
  requestExport(request: RequestAuditExportRequest): Observable<AuditExport> {
    return this.api.post<AuditExport>(`${this.endpoint}/exports`, request);
  }

  getExports(accountId: string): Observable<AuditExport[]> {
    return this.api.get<AuditExport[]>(`${this.endpoint}/exports/account/${accountId}`);
  }

  getExportById(exportId: string): Observable<AuditExport> {
    return this.api.get<AuditExport>(`${this.endpoint}/exports/${exportId}`);
  }

  downloadExport(exportId: string): Observable<Blob> {
    return this.api.get<Blob>(`${this.endpoint}/exports/${exportId}/download`);
  }

  export(filter: AuditLogFilter): Observable<Blob> {
    const params: Record<string, string> = {};
    if (filter.accountId) params['accountId'] = filter.accountId;
    if (filter.entityType) params['entityType'] = filter.entityType;
    if (filter.startDate) params['startDate'] = filter.startDate.toISOString();
    if (filter.endDate) params['endDate'] = filter.endDate.toISOString();
    return this.api.get<Blob>(`${this.endpoint}/logs/export`, params);
  }

  // Retention policies
  getRetentionPolicies(): Observable<RetentionPolicy[]> {
    return this.api.get<RetentionPolicy[]>(`${this.endpoint}/retention-policies`);
  }

  createRetentionPolicy(policyName: string, retentionDays: number): Observable<RetentionPolicy> {
    return this.api.post<RetentionPolicy>(`${this.endpoint}/retention-policies`, { policyName, retentionDays });
  }

  updateRetentionPolicy(policyId: string, retentionDays: number): Observable<RetentionPolicy> {
    return this.api.put<RetentionPolicy>(`${this.endpoint}/retention-policies/${policyId}`, { retentionDays });
  }

  activateRetentionPolicy(policyId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/retention-policies/${policyId}/activate`, {});
  }

  deactivateRetentionPolicy(policyId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/retention-policies/${policyId}/deactivate`, {});
  }

  applyRetentionPolicy(policyId: string): Observable<{ deletedCount: number }> {
    return this.api.post<{ deletedCount: number }>(`${this.endpoint}/retention-policies/${policyId}/apply`, {});
  }
}
