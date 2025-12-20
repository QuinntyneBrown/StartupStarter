import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Media, UploadMediaRequest, UpdateMediaRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class MediaService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'media';

  getAll(): Observable<Media[]> {
    return this.api.get<Media[]>(this.endpoint);
  }

  getById(mediaId: string): Observable<Media> {
    return this.api.get<Media>(`${this.endpoint}/${mediaId}`);
  }

  getByAccount(accountId: string): Observable<Media[]> {
    return this.api.get<Media[]>(`${this.endpoint}/account/${accountId}`);
  }

  getByProfile(profileId: string): Observable<Media[]> {
    return this.api.get<Media[]>(`${this.endpoint}/profile/${profileId}`);
  }

  getByTags(tags: string[]): Observable<Media[]> {
    return this.api.get<Media[]>(`${this.endpoint}/tags`, { tags: tags.join(',') });
  }

  getByCategory(category: string): Observable<Media[]> {
    return this.api.get<Media[]>(`${this.endpoint}/category/${category}`);
  }

  upload(request: UploadMediaRequest): Observable<Media> {
    const formData = new FormData();
    formData.append('file', request.file);
    formData.append('accountId', request.accountId);
    formData.append('profileId', request.profileId);
    if (request.tags) {
      formData.append('tags', JSON.stringify(request.tags));
    }
    if (request.categories) {
      formData.append('categories', JSON.stringify(request.categories));
    }
    return this.api.upload<Media>(this.endpoint, formData);
  }

  update(mediaId: string, request: UpdateMediaRequest): Observable<Media> {
    return this.api.put<Media>(`${this.endpoint}/${mediaId}`, request);
  }

  delete(mediaId: string, deletionType: 'SoftDelete' | 'HardDelete' = 'SoftDelete'): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${mediaId}?deletionType=${deletionType}`);
  }

  addTags(mediaId: string, tags: string[]): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${mediaId}/tags`, { tags });
  }

  removeTags(mediaId: string, tags: string[]): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${mediaId}/tags?tags=${tags.join(',')}`);
  }

  addCategories(mediaId: string, categories: string[]): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${mediaId}/categories`, { categories });
  }

  removeCategories(mediaId: string, categories: string[]): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${mediaId}/categories?categories=${categories.join(',')}`);
  }

  download(mediaId: string): Observable<Blob> {
    return this.api.get<Blob>(`${this.endpoint}/${mediaId}/download`);
  }

  getProcessingStatus(mediaId: string): Observable<{ status: string; progress: number }> {
    return this.api.get<{ status: string; progress: number }>(`${this.endpoint}/${mediaId}/processing-status`);
  }
}
