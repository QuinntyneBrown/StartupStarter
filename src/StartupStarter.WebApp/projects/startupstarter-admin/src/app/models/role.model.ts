export interface Role {
  roleId: string;
  roleName: string;
  description: string;
  accountId?: string;
  permissions: string[];
  isSystemRole: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface UserRole {
  userRoleId: string;
  roleId: string;
  userId: string;
  accountId: string;
  assignedBy: string;
  assignedAt: Date;
  revokedAt?: Date;
  revokedBy?: string;
  revocationReason?: string;
  isActive: boolean;
}

export interface CreateRoleRequest {
  roleName: string;
  description: string;
  permissions: string[];
}

export interface UpdateRoleRequest {
  roleName?: string;
  description?: string;
  permissions?: string[];
}

export interface AssignRoleRequest {
  userId: string;
  roleId: string;
}

export interface RevokeRoleRequest {
  userId: string;
  roleId: string;
  reason: string;
}

export const SYSTEM_PERMISSIONS = [
  'account:read', 'account:write', 'account:delete',
  'user:read', 'user:write', 'user:create', 'user:delete', 'user:activate', 'user:deactivate', 'user:lock',
  'role:read', 'role:write', 'role:create', 'role:delete', 'role:assign',
  'profile:read', 'profile:write', 'profile:create', 'profile:delete', 'profile:share',
  'content:read', 'content:write', 'content:create', 'content:delete', 'content:publish', 'content:approve',
  'media:read', 'media:write', 'media:upload', 'media:delete',
  'dashboard:read', 'dashboard:write', 'dashboard:create', 'dashboard:delete', 'dashboard:share',
  'workflow:read', 'workflow:write', 'workflow:approve', 'workflow:reject',
  'apikey:read', 'apikey:write', 'apikey:create', 'apikey:revoke',
  'webhook:read', 'webhook:write', 'webhook:create', 'webhook:delete',
  'audit:read', 'audit:export',
  'system:read', 'system:write', 'system:maintenance', 'system:backup'
];
