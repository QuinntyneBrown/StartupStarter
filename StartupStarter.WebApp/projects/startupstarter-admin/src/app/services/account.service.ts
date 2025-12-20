import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Account, CreateAccountRequest, UpdateAccountRequest, AccountSettings } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'accounts';

  getAll(): Observable<Account[]> {
    return this.api.get<Account[]>(this.endpoint);
  }

  getById(accountId: string): Observable<Account> {
    return this.api.get<Account>(`${this.endpoint}/${accountId}`);
  }

  getByOwner(ownerUserId: string): Observable<Account[]> {
    return this.api.get<Account[]>(`${this.endpoint}/owner/${ownerUserId}`);
  }

  create(request: CreateAccountRequest): Observable<Account> {
    return this.api.post<Account>(this.endpoint, request);
  }

  update(accountId: string, request: UpdateAccountRequest): Observable<Account> {
    return this.api.put<Account>(`${this.endpoint}/${accountId}`, request);
  }

  delete(accountId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${accountId}`);
  }

  suspend(accountId: string, reason: string, duration?: number): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${accountId}/suspend`, { reason, duration });
  }

  reactivate(accountId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${accountId}/reactivate`, {});
  }

  changeSubscriptionTier(accountId: string, newTier: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${accountId}/subscription`, { newTier });
  }

  transferOwnership(accountId: string, newOwnerUserId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${accountId}/transfer-ownership`, { newOwnerUserId });
  }

  getSettings(accountId: string): Observable<AccountSettings[]> {
    return this.api.get<AccountSettings[]>(`${this.endpoint}/${accountId}/settings`);
  }

  updateSettings(accountId: string, category: string, settings: Record<string, unknown>): Observable<boolean> {
    return this.api.put<boolean>(`${this.endpoint}/${accountId}/settings/${category}`, settings);
  }
}
