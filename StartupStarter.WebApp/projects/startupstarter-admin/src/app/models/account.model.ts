export enum AccountType {
  Individual = 'Individual',
  Team = 'Team',
  Enterprise = 'Enterprise'
}

export enum AccountStatus {
  Active = 'Active',
  Suspended = 'Suspended',
  Deleted = 'Deleted'
}

export interface Account {
  accountId: string;
  accountName: string;
  accountType: AccountType;
  ownerUserId: string;
  subscriptionTier: string;
  status: AccountStatus;
  createdAt: Date;
  updatedAt?: Date;
  deletedAt?: Date;
  suspendedAt?: Date;
  suspensionReason?: string;
}

export interface AccountSettings {
  accountSettingsId: string;
  accountId: string;
  category: string;
  settings: Record<string, unknown>;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateAccountRequest {
  accountName: string;
  accountType: AccountType;
  ownerUserId: string;
  subscriptionTier: string;
}

export interface UpdateAccountRequest {
  accountName?: string;
  subscriptionTier?: string;
}

export interface SuspendAccountRequest {
  reason: string;
  durationDays?: number;
}
