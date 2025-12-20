import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Media,
  UploadMediaRequest,
  UpdateMediaRequest,
  MediaSearchParams
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class MediaService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'media';

  getAll(): Observable<Media[]> {
    return this.api.get<Media[]>(this.endpoint);
  }

  getById(id: string): Observable<Media> {
    return this.api.get<Media>(`${this.endpoint}/${id}`);
  }

  upload(request: UploadMediaRequest): Observable<Media> {
    const additionalData: Record<string, string> = {};
    if (request.profileId) additionalData['profileId'] = request.profileId;
    if (request.tags) additionalData['tags'] = JSON.stringify(request.tags);
    if (request.categories) additionalData['categories'] = JSON.stringify(request.categories);

    return this.api.upload<Media>(`${this.endpoint}/upload`, request.file, additionalData);
  }

  update(id: string, request: UpdateMediaRequest): Observable<Media> {
    return this.api.put<Media>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  download(id: string): Observable<Blob> {
    return this.api.download(`${this.endpoint}/${id}/download`);
  }

  addTags(id: string, tags: string[]): Observable<Media> {
    return this.api.post<Media>(`${this.endpoint}/${id}/tags`, { tags });
  }

  removeTags(id: string, tags: string[]): Observable<Media> {
    return this.api.delete<Media>(`${this.endpoint}/${id}/tags`);
  }

  addCategories(id: string, categories: string[]): Observable<Media> {
    return this.api.post<Media>(`${this.endpoint}/${id}/categories`, { categories });
  }

  search(params: MediaSearchParams): Observable<Media[]> {
    return this.api.get<Media[]>(`${this.endpoint}/search`, params as Record<string, string | number | boolean>);
  }
}
