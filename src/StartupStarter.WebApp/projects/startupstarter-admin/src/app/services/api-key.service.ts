import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  ApiKey,
  CreateApiKeyRequest,
  CreateApiKeyResponse
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class ApiKeyService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'keys';

  getAll(): Observable<ApiKey[]> {
    return this.api.get<ApiKey[]>(this.endpoint);
  }

  getById(id: string): Observable<ApiKey> {
    return this.api.get<ApiKey>(`${this.endpoint}/${id}`);
  }

  create(request: CreateApiKeyRequest): Observable<CreateApiKeyResponse> {
    return this.api.post<CreateApiKeyResponse>(this.endpoint, request);
  }

  revoke(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  regenerate(id: string): Observable<CreateApiKeyResponse> {
    return this.api.post<CreateApiKeyResponse>(`${this.endpoint}/${id}/regenerate`, {});
  }
}
