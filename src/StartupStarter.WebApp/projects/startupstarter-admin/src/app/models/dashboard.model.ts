export enum LayoutType {
  Grid = 'Grid',
  Masonry = 'Masonry',
  Freeform = 'Freeform'
}

export enum CardType {
  Chart = 'Chart',
  Metric = 'Metric',
  Table = 'Table',
  Text = 'Text',
  Image = 'Image',
  Calendar = 'Calendar',
  Timeline = 'Timeline',
  Custom = 'Custom'
}

export interface Dashboard {
  dashboardId: string;
  dashboardName: string;
  profileId: string;
  accountId: string;
  createdBy: string;
  isDefault: boolean;
  layoutType: LayoutType;
  createdAt: Date;
  updatedAt?: Date;
  cards?: DashboardCard[];
}

export interface DashboardCard {
  cardId: string;
  dashboardId: string;
  cardType: CardType;
  title: string;
  configuration: Record<string, unknown>;
  row: number;
  column: number;
  width: number;
  height: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface DashboardShare {
  dashboardShareId: string;
  dashboardId: string;
  ownerUserId: string;
  sharedWithUserId: string;
  permissionLevel: string;
  sharedAt: Date;
}

export interface CreateDashboardRequest {
  dashboardName: string;
  layoutType: LayoutType;
}

export interface UpdateDashboardRequest {
  dashboardName?: string;
  layoutType?: LayoutType;
}

export interface AddCardRequest {
  cardType: CardType;
  title: string;
  configuration: Record<string, unknown>;
  row: number;
  column: number;
  width: number;
  height: number;
}

export interface UpdateCardPositionRequest {
  row: number;
  column: number;
  width: number;
  height: number;
}

export interface ShareDashboardRequest {
  userId: string;
  permissionLevel: string;
}
