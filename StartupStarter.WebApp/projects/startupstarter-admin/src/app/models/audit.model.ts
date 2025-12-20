export interface AuditLog {
  auditId: string;
  entityType: string;
  entityId: string;
  accountId: string;
  action: AuditAction;
  performedBy: string;
  userId: string;
  ipAddress: string;
  beforeState?: unknown;
  afterState?: unknown;
  timestamp: Date;
}

export enum AuditAction {
  Create = 'Create',
  Update = 'Update',
  Delete = 'Delete',
  View = 'View',
  Login = 'Login',
  Logout = 'Logout',
  LoginFailed = 'LoginFailed',
  PasswordChanged = 'PasswordChanged',
  PermissionGranted = 'PermissionGranted',
  PermissionRevoked = 'PermissionRevoked',
  Export = 'Export',
  Import = 'Import'
}

export interface AuditExport {
  exportId: string;
  accountId: string;
  requestedBy: string;
  startDate: Date;
  endDate: Date;
  filtersJson: string;
  fileFormat: FileFormat;
  status: ExportStatus;
  recordCount: number;
  fileLocation?: string;
  requestedAt: Date;
  completedAt?: Date;
}

export interface RetentionPolicy {
  retentionPolicyId: string;
  policyName: string;
  retentionDays: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export enum FileFormat {
  CSV = 'CSV',
  JSON = 'JSON',
  PDF = 'PDF'
}

export enum ExportStatus {
  Requested = 'Requested',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Failed = 'Failed'
}

export interface RequestAuditExportRequest {
  accountId: string;
  startDate: Date;
  endDate: Date;
  filters?: Record<string, unknown>;
  fileFormat: FileFormat;
}

export interface AuditLogFilter {
  accountId?: string;
  entityType?: string;
  entityId?: string;
  action?: string;
  startDate?: Date;
  endDate?: Date;
}
