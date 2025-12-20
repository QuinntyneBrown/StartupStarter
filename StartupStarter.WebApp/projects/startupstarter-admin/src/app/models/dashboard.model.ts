export interface Dashboard {
  dashboardId: string;
  dashboardName: string;
  profileId: string;
  accountId: string;
  createdBy: string;
  isDefault: boolean;
  template?: string;
  layoutType: LayoutType;
  createdAt: Date;
  updatedAt?: Date;
  cards: DashboardCard[];
  shares: DashboardShare[];
}

export interface DashboardCard {
  cardId: string;
  dashboardId: string;
  cardType: string;
  configurationJson: string;
  position: CardPosition;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CardPosition {
  row: number;
  column: number;
  width: number;
  height: number;
}

export interface DashboardShare {
  dashboardShareId: string;
  dashboardId: string;
  ownerUserId: string;
  sharedWithUserId: string;
  permissionLevel: DashboardPermissionLevel;
  sharedAt: Date;
}

export enum LayoutType {
  Grid = 'Grid',
  Masonry = 'Masonry',
  Freeform = 'Freeform'
}

export enum DashboardPermissionLevel {
  View = 'View',
  Edit = 'Edit'
}

export enum ExportFormat {
  JSON = 'JSON',
  PDF = 'PDF',
  Image = 'Image'
}

export interface CreateDashboardRequest {
  dashboardName: string;
  profileId: string;
  accountId: string;
  isDefault?: boolean;
  template?: string;
  layoutType?: LayoutType;
}

export interface UpdateDashboardRequest {
  dashboardName?: string;
  layoutType?: LayoutType;
}

export interface AddCardRequest {
  cardType: string;
  configurationJson: string;
  position: CardPosition;
}
