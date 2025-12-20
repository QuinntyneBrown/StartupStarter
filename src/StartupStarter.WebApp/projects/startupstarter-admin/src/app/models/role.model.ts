export interface Role {
  roleId: string;
  roleName: string;
  description: string;
  accountId: string;
  permissions: string[];
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
  accountId: string;
  permissions?: string[];
}

export interface UpdateRoleRequest {
  roleName?: string;
  description?: string;
  permissions?: string[];
}
