import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  User,
  UserInvitation,
  CreateUserRequest,
  UpdateUserRequest,
  SendInvitationRequest,
  ActivationMethod
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'users';

  getAll(): Observable<User[]> {
    return this.api.get<User[]>(this.endpoint);
  }

  getById(userId: string): Observable<User> {
    return this.api.get<User>(`${this.endpoint}/${userId}`);
  }

  getByAccount(accountId: string): Observable<User[]> {
    return this.api.get<User[]>(`${this.endpoint}/account/${accountId}`);
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.api.post<User>(this.endpoint, request);
  }

  update(userId: string, request: UpdateUserRequest): Observable<User> {
    return this.api.put<User>(`${this.endpoint}/${userId}`, request);
  }

  delete(userId: string, deletionType: 'SoftDelete' | 'HardDelete' = 'SoftDelete'): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${userId}?deletionType=${deletionType}`);
  }

  activate(userId: string, method: ActivationMethod = ActivationMethod.AdminActivation): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${userId}/activate`, { method });
  }

  deactivate(userId: string, reason: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${userId}/deactivate`, { reason });
  }

  lock(userId: string, reason: string, duration?: number): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${userId}/lock`, { reason, duration });
  }

  unlock(userId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${userId}/unlock`, {});
  }

  sendInvitation(request: SendInvitationRequest): Observable<UserInvitation> {
    return this.api.post<UserInvitation>(`${this.endpoint}/invitations`, request);
  }

  getInvitations(accountId: string): Observable<UserInvitation[]> {
    return this.api.get<UserInvitation[]>(`${this.endpoint}/invitations/account/${accountId}`);
  }

  resendInvitation(invitationId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/invitations/${invitationId}/resend`, {});
  }

  cancelInvitation(invitationId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/invitations/${invitationId}`);
  }

  assignRole(userId: string, roleId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${userId}/roles/${roleId}`, {});
  }

  removeRole(userId: string, roleId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${userId}/roles/${roleId}`);
  }
}
