export enum UserStatus {
  Invited = 'Invited',
  Active = 'Active',
  Inactive = 'Inactive',
  Locked = 'Locked',
  Deleted = 'Deleted'
}

export enum ActivationMethod {
  Email = 'Email',
  Admin = 'Admin',
  SSO = 'SSO'
}

export interface User {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  accountId: string;
  status: UserStatus;
  createdAt: Date;
  updatedAt?: Date;
  activatedAt?: Date;
  deactivatedAt?: Date;
  lockedAt?: Date;
  lockReason?: string;
  deletedAt?: Date;
  roles?: string[];
}

export interface UserInvitation {
  invitationId: string;
  email: string;
  accountId: string;
  invitedBy: string;
  sentAt: Date;
  expiresAt: Date;
  acceptedAt?: Date;
  isAccepted: boolean;
  isExpired: boolean;
  roleIds: string[];
}

export interface InviteUserRequest {
  email: string;
  firstName: string;
  lastName: string;
  roleIds: string[];
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
}

export interface DeactivateUserRequest {
  reason: string;
}

export interface LockUserRequest {
  reason: string;
  durationMinutes?: number;
}
