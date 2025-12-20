export interface SystemMaintenance {
  maintenanceId: string;
  scheduledStartTime: Date;
  estimatedDuration: number;
  maintenanceType: MaintenanceType;
  actualStartTime?: Date;
  completedTime?: Date;
  actualDuration?: number;
  affectedServices: string[];
}

export interface SystemBackup {
  backupId: string;
  backupType: BackupType;
  startedAt: Date;
  completedAt?: Date;
  backupSize?: number;
  duration?: number;
  backupLocation?: string;
  success: boolean;
  failureReason?: string;
}

export interface SystemError {
  errorId: string;
  errorMessage: string;
  stackTrace: string;
  severity: ErrorSeverity;
  component: string;
  occurredAt: Date;
  resolvedAt?: Date;
  resolvedBy?: string;
}

export enum MaintenanceType {
  Planned = 'Planned',
  Emergency = 'Emergency'
}

export enum BackupType {
  Full = 'Full',
  Incremental = 'Incremental',
  Differential = 'Differential'
}

export enum ErrorSeverity {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export interface ScheduleMaintenanceRequest {
  scheduledStartTime: Date;
  estimatedDuration: number;
  maintenanceType: MaintenanceType;
  affectedServices: string[];
}

export interface StartBackupRequest {
  backupType: BackupType;
}

export interface SystemConfig {
  applicationName: string;
  applicationUrl: string;
  supportEmail: string;
  defaultTimezone: string;
  defaultLanguage: string;
  maintenanceMode: boolean;
}
