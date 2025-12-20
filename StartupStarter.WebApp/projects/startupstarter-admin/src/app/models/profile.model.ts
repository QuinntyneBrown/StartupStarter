export interface Profile {
  profileId: string;
  profileName: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  bio?: string;
  accountId: string;
  userId?: string;
  createdBy: string;
  profileType: ProfileType;
  isDefault: boolean;
  avatarUrl?: string;
  createdAt: Date;
  updatedAt?: Date;
  dashboardIds: string[];
}

export interface ProfilePreferences {
  profilePreferencesId: string;
  profileId: string;
  category: string;
  preferencesJson: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface ProfileShare {
  profileShareId: string;
  profileId: string;
  ownerUserId: string;
  sharedWithUserId: string;
  permissionLevel: PermissionLevel;
  sharedAt: Date;
}

export enum ProfileType {
  Personal = 'Personal',
  Project = 'Project',
  Department = 'Department',
  Team = 'Team'
}

export enum PermissionLevel {
  View = 'View',
  Edit = 'Edit',
  Admin = 'Admin'
}

export interface CreateProfileRequest {
  profileName: string;
  accountId: string;
  profileType: ProfileType;
  isDefault?: boolean;
}

export interface UpdateProfileRequest {
  profileName?: string;
  profileType?: ProfileType;
}
