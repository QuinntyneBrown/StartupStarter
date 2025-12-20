import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Dashboard,
  DashboardCard,
  CreateDashboardRequest,
  UpdateDashboardRequest,
  AddCardRequest,
  CardPosition,
  LayoutType,
  DashboardPermissionLevel
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'dashboards';

  getAll(): Observable<Dashboard[]> {
    return this.api.get<Dashboard[]>(this.endpoint);
  }

  getById(dashboardId: string): Observable<Dashboard> {
    return this.api.get<Dashboard>(`${this.endpoint}/${dashboardId}`);
  }

  getByProfile(profileId: string): Observable<Dashboard[]> {
    return this.api.get<Dashboard[]>(`${this.endpoint}/profile/${profileId}`);
  }

  getByAccount(accountId: string): Observable<Dashboard[]> {
    return this.api.get<Dashboard[]>(`${this.endpoint}/account/${accountId}`);
  }

  create(request: CreateDashboardRequest): Observable<Dashboard> {
    return this.api.post<Dashboard>(this.endpoint, request);
  }

  update(dashboardId: string, request: UpdateDashboardRequest): Observable<Dashboard> {
    return this.api.put<Dashboard>(`${this.endpoint}/${dashboardId}`, request);
  }

  delete(dashboardId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${dashboardId}`);
  }

  clone(dashboardId: string, newName: string): Observable<Dashboard> {
    return this.api.post<Dashboard>(`${this.endpoint}/${dashboardId}/clone`, { newName });
  }

  setAsDefault(dashboardId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${dashboardId}/set-default`, {});
  }

  changeLayout(dashboardId: string, layoutType: LayoutType): Observable<boolean> {
    return this.api.put<boolean>(`${this.endpoint}/${dashboardId}/layout`, { layoutType });
  }

  share(dashboardId: string, userIds: string[], permissionLevel: DashboardPermissionLevel): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${dashboardId}/share`, { userIds, permissionLevel });
  }

  revokeShare(dashboardId: string, userIds: string[]): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${dashboardId}/revoke-share`, { userIds });
  }

  moveToProfile(dashboardId: string, newProfileId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${dashboardId}/move`, { newProfileId });
  }

  // Card operations
  getCards(dashboardId: string): Observable<DashboardCard[]> {
    return this.api.get<DashboardCard[]>(`${this.endpoint}/${dashboardId}/cards`);
  }

  addCard(dashboardId: string, request: AddCardRequest): Observable<DashboardCard> {
    return this.api.post<DashboardCard>(`${this.endpoint}/${dashboardId}/cards`, request);
  }

  updateCard(dashboardId: string, cardId: string, config: Record<string, unknown>): Observable<DashboardCard> {
    return this.api.put<DashboardCard>(`${this.endpoint}/${dashboardId}/cards/${cardId}`, config);
  }

  removeCard(dashboardId: string, cardId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${dashboardId}/cards/${cardId}`);
  }

  repositionCard(dashboardId: string, cardId: string, position: CardPosition): Observable<boolean> {
    return this.api.put<boolean>(`${this.endpoint}/${dashboardId}/cards/${cardId}/position`, position);
  }

  // Export/Import
  export(dashboardId: string, format: 'JSON' | 'PDF' | 'Image'): Observable<Blob> {
    return this.api.get<Blob>(`${this.endpoint}/${dashboardId}/export?format=${format}`);
  }

  import(profileId: string, dashboardData: unknown): Observable<Dashboard> {
    return this.api.post<Dashboard>(`${this.endpoint}/import`, { profileId, dashboardData });
  }
}
