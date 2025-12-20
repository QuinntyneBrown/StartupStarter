export interface Account {
  accountId: string;
  accountName: string;
  accountType: AccountType;
  ownerUserId: string;
  subscriptionTier: string;
  status: AccountStatus;
  createdAt: Date;
  updatedAt?: Date;
}

export interface AccountSettings {
  accountSettingsId: string;
  accountId: string;
  category: string;
  settingsJson: string;
  createdAt: Date;
  updatedAt?: Date;
}

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

export interface CreateAccountRequest {
  accountName: string;
  accountType: AccountType;
  ownerUserId: string;
  subscriptionTier: string;
}

export interface UpdateAccountRequest {
  accountName?: string;
  updatedFields?: Record<string, unknown>;
}
