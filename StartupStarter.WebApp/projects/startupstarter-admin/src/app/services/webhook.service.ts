import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Webhook, WebhookDelivery, RegisterWebhookRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class WebhookService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'webhooks';

  getAll(): Observable<Webhook[]> {
    return this.api.get<Webhook[]>(this.endpoint);
  }

  getById(webhookId: string): Observable<Webhook> {
    return this.api.get<Webhook>(`${this.endpoint}/${webhookId}`);
  }

  getByAccount(accountId: string): Observable<Webhook[]> {
    return this.api.get<Webhook[]>(`${this.endpoint}/account/${accountId}`);
  }

  register(request: RegisterWebhookRequest): Observable<Webhook> {
    return this.api.post<Webhook>(this.endpoint, request);
  }

  update(webhookId: string, url: string, events: string[]): Observable<Webhook> {
    return this.api.put<Webhook>(`${this.endpoint}/${webhookId}`, { url, events });
  }

  delete(webhookId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${webhookId}`);
  }

  addEvent(webhookId: string, eventType: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${webhookId}/events`, { eventType });
  }

  removeEvent(webhookId: string, eventType: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${webhookId}/events/${eventType}`);
  }

  // Delivery operations
  getDeliveries(webhookId: string, startDate?: Date, endDate?: Date): Observable<WebhookDelivery[]> {
    const params: Record<string, string> = {};
    if (startDate) params['startDate'] = startDate.toISOString();
    if (endDate) params['endDate'] = endDate.toISOString();
    return this.api.get<WebhookDelivery[]>(`${this.endpoint}/${webhookId}/deliveries`, params);
  }

  retryDelivery(webhookId: string, deliveryId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${webhookId}/deliveries/${deliveryId}/retry`, {});
  }

  testWebhook(webhookId: string): Observable<WebhookDelivery> {
    return this.api.post<WebhookDelivery>(`${this.endpoint}/${webhookId}/test`, {});
  }

  test(webhookId: string): Observable<WebhookDelivery> {
    return this.testWebhook(webhookId);
  }

  enable(webhookId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${webhookId}/enable`, {});
  }

  disable(webhookId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${webhookId}/disable`, {});
  }

  getAvailableEvents(): Observable<string[]> {
    return this.api.get<string[]>(`${this.endpoint}/available-events`);
  }
}
