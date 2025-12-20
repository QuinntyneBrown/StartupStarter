import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  User,
  UserInvitation,
  InviteUserRequest,
  UpdateUserRequest,
  DeactivateUserRequest,
  LockUserRequest
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

  getById(id: string): Observable<User> {
    return this.api.get<User>(`${this.endpoint}/${id}`);
  }

  invite(request: InviteUserRequest): Observable<UserInvitation> {
    return this.api.post<UserInvitation>(`${this.endpoint}/invite`, request);
  }

  update(id: string, request: UpdateUserRequest): Observable<User> {
    return this.api.put<User>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  activate(id: string): Observable<User> {
    return this.api.post<User>(`${this.endpoint}/${id}/activate`, {});
  }

  deactivate(id: string, request: DeactivateUserRequest): Observable<User> {
    return this.api.post<User>(`${this.endpoint}/${id}/deactivate`, request);
  }

  lock(id: string, request: LockUserRequest): Observable<User> {
    return this.api.post<User>(`${this.endpoint}/${id}/lock`, request);
  }

  unlock(id: string): Observable<User> {
    return this.api.post<User>(`${this.endpoint}/${id}/unlock`, {});
  }

  getRoles(id: string): Observable<string[]> {
    return this.api.get<string[]>(`${this.endpoint}/${id}/roles`);
  }

  getInvitations(): Observable<UserInvitation[]> {
    return this.api.get<UserInvitation[]>('invitations');
  }

  cancelInvitation(id: string): Observable<void> {
    return this.api.delete<void>(`invitations/${id}`);
  }

  resendInvitation(id: string): Observable<UserInvitation> {
    return this.api.post<UserInvitation>(`invitations/${id}/resend`, {});
  }
}
