import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  AuditLog,
  AuditExport,
  AuditSearchParams,
  RequestExportRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuditService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'audit';

  getLogs(params?: AuditSearchParams): Observable<AuditLog[]> {
    return this.api.get<AuditLog[]>(`${this.endpoint}/logs`, params as Record<string, string | number | boolean>);
  }

  getLogById(id: string): Observable<AuditLog> {
    return this.api.get<AuditLog>(`${this.endpoint}/logs/${id}`);
  }

  requestExport(request: RequestExportRequest): Observable<AuditExport> {
    return this.api.post<AuditExport>(`${this.endpoint}/export`, request);
  }

  getExport(id: string): Observable<AuditExport> {
    return this.api.get<AuditExport>(`${this.endpoint}/export/${id}`);
  }

  downloadExport(id: string): Observable<Blob> {
    return this.api.download(`${this.endpoint}/export/${id}/download`);
  }

  getExports(): Observable<AuditExport[]> {
    return this.api.get<AuditExport[]>(`${this.endpoint}/exports`);
  }
}
