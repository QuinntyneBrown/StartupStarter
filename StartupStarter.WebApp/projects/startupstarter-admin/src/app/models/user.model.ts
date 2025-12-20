export interface User {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  accountId: string;
  status: UserStatus;
  roleIds: string[];
  createdAt: Date;
  updatedAt?: Date;
  activatedAt?: Date;
  deactivatedAt?: Date;
  lockedAt?: Date;
  lockReason?: string;
}

export interface UserInvitation {
  invitationId: string;
  email: string;
  accountId: string;
  invitedBy: string;
  sentAt: Date;
  expiresAt: Date;
  acceptedAt?: Date;
  acceptedByUserId?: string;
  isAccepted: boolean;
  isExpired: boolean;
  roleIds: string[];
}

export enum UserStatus {
  Invited = 'Invited',
  Active = 'Active',
  Inactive = 'Inactive',
  Locked = 'Locked',
  Deleted = 'Deleted'
}

export enum ActivationMethod {
  EmailVerification = 'EmailVerification',
  AdminActivation = 'AdminActivation',
  AutoActivation = 'AutoActivation'
}

export interface CreateUserRequest {
  email: string;
  firstName: string;
  lastName: string;
  accountId: string;
  roleIds?: string[];
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  roleIds?: string[];
}

export interface SendInvitationRequest {
  email: string;
  accountId: string;
  roleIds?: string[];
}
