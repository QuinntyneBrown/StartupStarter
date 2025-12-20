export enum AuditAction {
  Create = 'Create',
  Update = 'Update',
  Delete = 'Delete',
  Suspend = 'Suspend',
  Activate = 'Activate',
  Deactivate = 'Deactivate',
  Lock = 'Lock',
  Unlock = 'Unlock',
  Login = 'Login',
  Logout = 'Logout',
  PasswordReset = 'PasswordReset',
  PermissionChange = 'PermissionChange',
  Export = 'Export'
}

export interface AuditLog {
  auditLogId: string;
  entityType: string;
  entityId: string;
  action: AuditAction;
  accountId: string;
  userId: string;
  userName?: string;
  ipAddress?: string;
  userAgent?: string;
  beforeState?: Record<string, unknown>;
  afterState?: Record<string, unknown>;
  description?: string;
  timestamp: Date;
}

export interface AuditExport {
  auditExportId: string;
  accountId: string;
  requestedBy: string;
  format: ExportFormat;
  filters: AuditSearchParams;
  status: ExportStatus;
  downloadUrl?: string;
  requestedAt: Date;
  completedAt?: Date;
  expiresAt?: Date;
}

export enum ExportFormat {
  CSV = 'CSV',
  JSON = 'JSON',
  PDF = 'PDF'
}

export enum ExportStatus {
  Pending = 'Pending',
  Processing = 'Processing',
  Completed = 'Completed',
  Failed = 'Failed'
}

export interface AuditSearchParams {
  entityType?: string;
  entityId?: string;
  action?: AuditAction;
  userId?: string;
  startDate?: Date;
  endDate?: Date;
  page?: number;
  pageSize?: number;
}

export interface RequestExportRequest {
  format: ExportFormat;
  filters: AuditSearchParams;
}
