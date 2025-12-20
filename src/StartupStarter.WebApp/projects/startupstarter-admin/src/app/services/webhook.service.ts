import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Webhook,
  WebhookDelivery,
  CreateWebhookRequest,
  UpdateWebhookRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class WebhookService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'webhooks';

  getAll(): Observable<Webhook[]> {
    return this.api.get<Webhook[]>(this.endpoint);
  }

  getById(id: string): Observable<Webhook> {
    return this.api.get<Webhook>(`${this.endpoint}/${id}`);
  }

  create(request: CreateWebhookRequest): Observable<Webhook> {
    return this.api.post<Webhook>(this.endpoint, request);
  }

  update(id: string, request: UpdateWebhookRequest): Observable<Webhook> {
    return this.api.put<Webhook>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  enable(id: string): Observable<Webhook> {
    return this.api.post<Webhook>(`${this.endpoint}/${id}/enable`, {});
  }

  disable(id: string): Observable<Webhook> {
    return this.api.post<Webhook>(`${this.endpoint}/${id}/disable`, {});
  }

  getDeliveries(id: string): Observable<WebhookDelivery[]> {
    return this.api.get<WebhookDelivery[]>(`${this.endpoint}/${id}/deliveries`);
  }

  retryDelivery(webhookId: string, deliveryId: string): Observable<WebhookDelivery> {
    return this.api.post<WebhookDelivery>(`${this.endpoint}/${webhookId}/deliveries/${deliveryId}/retry`, {});
  }

  test(id: string): Observable<WebhookDelivery> {
    return this.api.post<WebhookDelivery>(`${this.endpoint}/${id}/test`, {});
  }
}
