import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Profile,
  ProfilePreferences,
  ProfileShare,
  CreateProfileRequest,
  UpdateProfileRequest,
  ShareProfileRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'profiles';

  getAll(): Observable<Profile[]> {
    return this.api.get<Profile[]>(this.endpoint);
  }

  getById(id: string): Observable<Profile> {
    return this.api.get<Profile>(`${this.endpoint}/${id}`);
  }

  create(request: CreateProfileRequest): Observable<Profile> {
    return this.api.post<Profile>(this.endpoint, request);
  }

  update(id: string, request: UpdateProfileRequest): Observable<Profile> {
    return this.api.put<Profile>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  updateAvatar(id: string, file: File): Observable<Profile> {
    return this.api.upload<Profile>(`${this.endpoint}/${id}/avatar`, file);
  }

  getPreferences(id: string): Observable<ProfilePreferences[]> {
    return this.api.get<ProfilePreferences[]>(`${this.endpoint}/${id}/preferences`);
  }

  updatePreferences(id: string, category: string, preferences: Record<string, unknown>): Observable<ProfilePreferences> {
    return this.api.put<ProfilePreferences>(`${this.endpoint}/${id}/preferences`, { category, preferences });
  }

  share(id: string, request: ShareProfileRequest): Observable<ProfileShare[]> {
    return this.api.post<ProfileShare[]>(`${this.endpoint}/${id}/share`, request);
  }

  revokeShare(id: string, userId: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}/share/${userId}`);
  }

  setAsDefault(id: string): Observable<Profile> {
    return this.api.post<Profile>(`${this.endpoint}/${id}/default`, {});
  }

  getShares(id: string): Observable<ProfileShare[]> {
    return this.api.get<ProfileShare[]>(`${this.endpoint}/${id}/shares`);
  }
}
