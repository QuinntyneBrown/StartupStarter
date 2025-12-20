import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Role, CreateRoleRequest, UpdateRoleRequest, UserRole } from '../models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'roles';

  getAll(): Observable<Role[]> {
    return this.api.get<Role[]>(this.endpoint);
  }

  getById(roleId: string): Observable<Role> {
    return this.api.get<Role>(`${this.endpoint}/${roleId}`);
  }

  getByAccount(accountId: string): Observable<Role[]> {
    return this.api.get<Role[]>(`${this.endpoint}/account/${accountId}`);
  }

  create(request: CreateRoleRequest): Observable<Role> {
    return this.api.post<Role>(this.endpoint, request);
  }

  update(roleId: string, request: UpdateRoleRequest): Observable<Role> {
    return this.api.put<Role>(`${this.endpoint}/${roleId}`, request);
  }

  delete(roleId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${roleId}`);
  }

  updatePermissions(roleId: string, permissions: string[]): Observable<boolean> {
    return this.api.put<boolean>(`${this.endpoint}/${roleId}/permissions`, { permissions });
  }

  addPermission(roleId: string, permission: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${roleId}/permissions`, { permission });
  }

  removePermission(roleId: string, permission: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${roleId}/permissions/${permission}`);
  }

  getUsersWithRole(roleId: string): Observable<UserRole[]> {
    return this.api.get<UserRole[]>(`${this.endpoint}/${roleId}/users`);
  }

  assignToUser(roleId: string, userId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${roleId}/users/${userId}`, {});
  }

  revokeFromUser(roleId: string, userId: string, reason?: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${roleId}/users/${userId}${reason ? `?reason=${reason}` : ''}`);
  }

  getAvailablePermissions(): Observable<string[]> {
    return this.api.get<string[]>(`${this.endpoint}/permissions`);
  }
}
