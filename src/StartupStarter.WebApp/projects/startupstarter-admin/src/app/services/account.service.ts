import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Account,
  AccountSettings,
  CreateAccountRequest,
  UpdateAccountRequest,
  SuspendAccountRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'accounts';

  getAll(): Observable<Account[]> {
    return this.api.get<Account[]>(this.endpoint);
  }

  getById(id: string): Observable<Account> {
    return this.api.get<Account>(`${this.endpoint}/${id}`);
  }

  getByOwner(userId: string): Observable<Account[]> {
    return this.api.get<Account[]>(`${this.endpoint}/owner/${userId}`);
  }

  create(request: CreateAccountRequest): Observable<Account> {
    return this.api.post<Account>(this.endpoint, request);
  }

  update(id: string, request: UpdateAccountRequest): Observable<Account> {
    return this.api.put<Account>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  suspend(id: string, request: SuspendAccountRequest): Observable<Account> {
    return this.api.post<Account>(`${this.endpoint}/${id}/suspend`, request);
  }

  reactivate(id: string): Observable<Account> {
    return this.api.post<Account>(`${this.endpoint}/${id}/reactivate`, {});
  }

  changeSubscription(id: string, subscriptionTier: string): Observable<Account> {
    return this.api.put<Account>(`${this.endpoint}/${id}/subscription`, { subscriptionTier });
  }

  transferOwnership(id: string, newOwnerId: string): Observable<Account> {
    return this.api.put<Account>(`${this.endpoint}/${id}/owner`, { newOwnerId });
  }

  getSettings(id: string): Observable<AccountSettings[]> {
    return this.api.get<AccountSettings[]>(`${this.endpoint}/${id}/settings`);
  }

  updateSettings(id: string, settings: Partial<AccountSettings>): Observable<AccountSettings> {
    return this.api.put<AccountSettings>(`${this.endpoint}/${id}/settings`, settings);
  }
}
