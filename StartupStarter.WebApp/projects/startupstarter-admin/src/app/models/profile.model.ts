export enum ProfileType {
  Personal = 'Personal',
  Project = 'Project',
  Department = 'Department',
  Team = 'Team'
}

export enum SharePermissionLevel {
  View = 'View',
  Edit = 'Edit',
  Admin = 'Admin'
}

export interface Profile {
  profileId: string;
  profileName: string;
  accountId: string;
  createdBy: string;
  profileType: ProfileType;
  isDefault: boolean;
  avatarUrl?: string;
  createdAt: Date;
  updatedAt?: Date;
  deletedAt?: Date;
}

export interface ProfilePreferences {
  profilePreferencesId: string;
  profileId: string;
  category: string;
  preferences: Record<string, unknown>;
  createdAt: Date;
  updatedAt?: Date;
}

export interface ProfileShare {
  profileShareId: string;
  profileId: string;
  ownerUserId: string;
  sharedWithUserId: string;
  permissionLevel: SharePermissionLevel;
  sharedAt: Date;
}

export interface CreateProfileRequest {
  profileName: string;
  profileType: ProfileType;
}

export interface UpdateProfileRequest {
  profileName?: string;
}

export interface ShareProfileRequest {
  userIds: string[];
  permissionLevel: SharePermissionLevel;
}
