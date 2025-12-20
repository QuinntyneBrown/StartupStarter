import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Profile,
  ProfilePreferences,
  CreateProfileRequest,
  UpdateProfileRequest,
  PermissionLevel
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

  getById(profileId: string): Observable<Profile> {
    return this.api.get<Profile>(`${this.endpoint}/${profileId}`);
  }

  getByAccount(accountId: string): Observable<Profile[]> {
    return this.api.get<Profile[]>(`${this.endpoint}/account/${accountId}`);
  }

  getByUserId(userId: string): Observable<Profile> {
    return this.api.get<Profile>(`${this.endpoint}/user/${userId}`);
  }

  create(request: CreateProfileRequest): Observable<Profile> {
    return this.api.post<Profile>(this.endpoint, request);
  }

  update(profileId: string, request: UpdateProfileRequest): Observable<Profile> {
    return this.api.put<Profile>(`${this.endpoint}/${profileId}`, request);
  }

  delete(profileId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${profileId}`);
  }

  setAsDefault(profileId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${profileId}/set-default`, {});
  }

  updateAvatar(profileId: string, avatarFile: File): Observable<Profile> {
    const formData = new FormData();
    formData.append('avatar', avatarFile);
    return this.api.upload<Profile>(`${this.endpoint}/${profileId}/avatar`, formData);
  }

  getPreferences(profileId: string): Observable<ProfilePreferences[]> {
    return this.api.get<ProfilePreferences[]>(`${this.endpoint}/${profileId}/preferences`);
  }

  updatePreferences(profileId: string, category: string, preferences: Record<string, unknown>): Observable<boolean> {
    return this.api.put<boolean>(`${this.endpoint}/${profileId}/preferences/${category}`, preferences);
  }

  share(profileId: string, userIds: string[], permissionLevel: PermissionLevel): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${profileId}/share`, { userIds, permissionLevel });
  }

  revokeShare(profileId: string, userIds: string[]): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${profileId}/revoke-share`, { userIds });
  }

  addDashboard(profileId: string, dashboardId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${profileId}/dashboards/${dashboardId}`, {});
  }

  removeDashboard(profileId: string, dashboardId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${profileId}/dashboards/${dashboardId}`);
  }
}
