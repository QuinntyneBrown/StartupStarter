import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Role,
  UserRole,
  CreateRoleRequest,
  UpdateRoleRequest,
  AssignRoleRequest,
  RevokeRoleRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'roles';

  getAll(): Observable<Role[]> {
    return this.api.get<Role[]>(this.endpoint);
  }

  getById(id: string): Observable<Role> {
    return this.api.get<Role>(`${this.endpoint}/${id}`);
  }

  create(request: CreateRoleRequest): Observable<Role> {
    return this.api.post<Role>(this.endpoint, request);
  }

  update(id: string, request: UpdateRoleRequest): Observable<Role> {
    return this.api.put<Role>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  updatePermissions(id: string, permissions: string[]): Observable<Role> {
    return this.api.put<Role>(`${this.endpoint}/${id}/permissions`, { permissions });
  }

  assignToUser(request: AssignRoleRequest): Observable<UserRole> {
    return this.api.post<UserRole>(`${this.endpoint}/${request.roleId}/assign`, request);
  }

  revokeFromUser(request: RevokeRoleRequest): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${request.roleId}/revoke`, request);
  }

  getUsersWithRole(roleId: string): Observable<string[]> {
    return this.api.get<string[]>(`${this.endpoint}/${roleId}/users`);
  }

  getUserRoles(userId: string): Observable<Role[]> {
    return this.api.get<Role[]>(`users/${userId}/roles`);
  }

  getUserPermissions(userId: string): Observable<string[]> {
    return this.api.get<string[]>(`users/${userId}/permissions`);
  }
}
