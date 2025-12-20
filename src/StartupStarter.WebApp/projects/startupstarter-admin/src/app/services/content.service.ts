import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Content,
  ContentVersion,
  CreateContentRequest,
  UpdateContentRequest,
  PublishContentRequest,
  UnpublishContentRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class ContentService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'content';

  getAll(): Observable<Content[]> {
    return this.api.get<Content[]>(this.endpoint);
  }

  getById(id: string): Observable<Content> {
    return this.api.get<Content>(`${this.endpoint}/${id}`);
  }

  create(request: CreateContentRequest): Observable<Content> {
    return this.api.post<Content>(this.endpoint, request);
  }

  update(id: string, request: UpdateContentRequest): Observable<Content> {
    return this.api.put<Content>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  publish(id: string, request?: PublishContentRequest): Observable<Content> {
    return this.api.post<Content>(`${this.endpoint}/${id}/publish`, request || {});
  }

  unpublish(id: string, request: UnpublishContentRequest): Observable<Content> {
    return this.api.post<Content>(`${this.endpoint}/${id}/unpublish`, request);
  }

  getVersions(id: string): Observable<ContentVersion[]> {
    return this.api.get<ContentVersion[]>(`${this.endpoint}/${id}/versions`);
  }

  getVersion(id: string, versionId: string): Observable<ContentVersion> {
    return this.api.get<ContentVersion>(`${this.endpoint}/${id}/versions/${versionId}`);
  }

  restoreVersion(id: string, versionNumber: number): Observable<Content> {
    return this.api.post<Content>(`${this.endpoint}/${id}/restore/${versionNumber}`, {});
  }

  schedule(id: string, scheduledDate: Date): Observable<Content> {
    return this.api.post<Content>(`${this.endpoint}/${id}/schedule`, { scheduledDate });
  }

  cancelSchedule(id: string): Observable<Content> {
    return this.api.post<Content>(`${this.endpoint}/${id}/cancel-schedule`, {});
  }
}
