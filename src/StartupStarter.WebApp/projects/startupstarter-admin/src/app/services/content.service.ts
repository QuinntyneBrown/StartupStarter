import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Content,
  ContentVersion,
  ContentStatus,
  CreateContentRequest,
  UpdateContentRequest,
  ScheduleContentRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class ContentService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'contents';

  getAll(): Observable<Content[]> {
    return this.api.get<Content[]>(this.endpoint);
  }

  getById(contentId: string): Observable<Content> {
    return this.api.get<Content>(`${this.endpoint}/${contentId}`);
  }

  getByAccount(accountId: string): Observable<Content[]> {
    return this.api.get<Content[]>(`${this.endpoint}/account/${accountId}`);
  }

  getByProfile(profileId: string): Observable<Content[]> {
    return this.api.get<Content[]>(`${this.endpoint}/profile/${profileId}`);
  }

  getByStatus(status: ContentStatus): Observable<Content[]> {
    return this.api.get<Content[]>(`${this.endpoint}/status/${status}`);
  }

  create(request: CreateContentRequest): Observable<Content> {
    return this.api.post<Content>(this.endpoint, request);
  }

  update(contentId: string, request: UpdateContentRequest): Observable<Content> {
    return this.api.put<Content>(`${this.endpoint}/${contentId}`, request);
  }

  delete(contentId: string, deletionType: 'SoftDelete' | 'HardDelete' = 'SoftDelete'): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${contentId}?deletionType=${deletionType}`);
  }

  publish(contentId: string, publishDate?: Date): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${contentId}/publish`, { publishDate });
  }

  unpublish(contentId: string, reason: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${contentId}/unpublish`, { reason });
  }

  changeStatus(contentId: string, newStatus: ContentStatus): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${contentId}/status`, { newStatus });
  }

  schedule(contentId: string, request: ScheduleContentRequest): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${contentId}/schedule`, request);
  }

  cancelSchedule(contentId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${contentId}/cancel-schedule`, {});
  }

  // Version operations
  getVersions(contentId: string): Observable<ContentVersion[]> {
    return this.api.get<ContentVersion[]>(`${this.endpoint}/${contentId}/versions`);
  }

  getVersion(contentId: string, versionNumber: number): Observable<ContentVersion> {
    return this.api.get<ContentVersion>(`${this.endpoint}/${contentId}/versions/${versionNumber}`);
  }

  createVersion(contentId: string, changeDescription: string): Observable<ContentVersion> {
    return this.api.post<ContentVersion>(`${this.endpoint}/${contentId}/versions`, { changeDescription });
  }

  restoreVersion(contentId: string, versionNumber: number): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${contentId}/versions/${versionNumber}/restore`, {});
  }
}
