import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Dashboard,
  DashboardCard,
  DashboardShare,
  CreateDashboardRequest,
  UpdateDashboardRequest,
  AddCardRequest,
  UpdateCardPositionRequest,
  ShareDashboardRequest
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

  getById(id: string): Observable<Dashboard> {
    return this.api.get<Dashboard>(`${this.endpoint}/${id}`);
  }

  create(request: CreateDashboardRequest): Observable<Dashboard> {
    return this.api.post<Dashboard>(this.endpoint, request);
  }

  update(id: string, request: UpdateDashboardRequest): Observable<Dashboard> {
    return this.api.put<Dashboard>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  clone(id: string, newName: string): Observable<Dashboard> {
    return this.api.post<Dashboard>(`${this.endpoint}/${id}/clone`, { newName });
  }

  setAsDefault(id: string): Observable<Dashboard> {
    return this.api.post<Dashboard>(`${this.endpoint}/${id}/default`, {});
  }

  share(id: string, request: ShareDashboardRequest): Observable<DashboardShare> {
    return this.api.post<DashboardShare>(`${this.endpoint}/${id}/share`, request);
  }

  revokeShare(id: string, userId: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}/share/${userId}`);
  }

  getCards(id: string): Observable<DashboardCard[]> {
    return this.api.get<DashboardCard[]>(`${this.endpoint}/${id}/cards`);
  }

  addCard(id: string, request: AddCardRequest): Observable<DashboardCard> {
    return this.api.post<DashboardCard>(`${this.endpoint}/${id}/cards`, request);
  }

  updateCard(dashboardId: string, cardId: string, request: Partial<AddCardRequest>): Observable<DashboardCard> {
    return this.api.put<DashboardCard>(`${this.endpoint}/${dashboardId}/cards/${cardId}`, request);
  }

  updateCardPosition(dashboardId: string, cardId: string, request: UpdateCardPositionRequest): Observable<DashboardCard> {
    return this.api.put<DashboardCard>(`${this.endpoint}/${dashboardId}/cards/${cardId}/position`, request);
  }

  removeCard(dashboardId: string, cardId: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${dashboardId}/cards/${cardId}`);
  }

  exportDashboard(id: string): Observable<Blob> {
    return this.api.download(`${this.endpoint}/${id}/export`);
  }

  importDashboard(file: File): Observable<Dashboard> {
    return this.api.upload<Dashboard>(`${this.endpoint}/import`, file);
  }
}
