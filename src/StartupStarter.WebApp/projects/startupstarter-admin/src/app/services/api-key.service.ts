import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ApiKey, ApiRequest, CreateApiKeyRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ApiKeyService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'apikeys';

  getAll(): Observable<ApiKey[]> {
    return this.api.get<ApiKey[]>(this.endpoint);
  }

  getById(apiKeyId: string): Observable<ApiKey> {
    return this.api.get<ApiKey>(`${this.endpoint}/${apiKeyId}`);
  }

  getByAccount(accountId: string): Observable<ApiKey[]> {
    return this.api.get<ApiKey[]>(`${this.endpoint}/account/${accountId}`);
  }

  create(request: CreateApiKeyRequest): Observable<{ apiKey: ApiKey; rawKey: string }> {
    return this.api.post<{ apiKey: ApiKey; rawKey: string }>(this.endpoint, request);
  }

  revoke(apiKeyId: string, reason: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${apiKeyId}/revoke`, { reason });
  }

  updatePermissions(apiKeyId: string, permissions: string[]): Observable<boolean> {
    return this.api.put<boolean>(`${this.endpoint}/${apiKeyId}/permissions`, { permissions });
  }

  addPermission(apiKeyId: string, permission: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${apiKeyId}/permissions`, { permission });
  }

  removePermission(apiKeyId: string, permission: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${apiKeyId}/permissions/${permission}`);
  }

  // API Request history
  getRequests(apiKeyId: string, startDate?: Date, endDate?: Date): Observable<ApiRequest[]> {
    const params: Record<string, string> = {};
    if (startDate) params['startDate'] = startDate.toISOString();
    if (endDate) params['endDate'] = endDate.toISOString();
    return this.api.get<ApiRequest[]>(`${this.endpoint}/${apiKeyId}/requests`, params);
  }

  getRequestsByAccount(accountId: string, startDate?: Date, endDate?: Date): Observable<ApiRequest[]> {
    const params: Record<string, string> = {};
    if (startDate) params['startDate'] = startDate.toISOString();
    if (endDate) params['endDate'] = endDate.toISOString();
    return this.api.get<ApiRequest[]>(`${this.endpoint}/requests/account/${accountId}`, params);
  }

  getAvailablePermissions(): Observable<string[]> {
    return this.api.get<string[]>(`${this.endpoint}/available-permissions`);
  }

  delete(apiKeyId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${apiKeyId}`);
  }
}
