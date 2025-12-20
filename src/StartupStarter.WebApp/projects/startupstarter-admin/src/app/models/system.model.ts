export enum MaintenanceType {
  Scheduled = 'Scheduled',
  Emergency = 'Emergency'
}

export enum MaintenanceStatus {
  Scheduled = 'Scheduled',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum BackupType {
  Full = 'Full',
  Incremental = 'Incremental',
  Differential = 'Differential'
}

export enum BackupStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Failed = 'Failed'
}

export enum ErrorSeverity {
  Critical = 'Critical',
  High = 'High',
  Medium = 'Medium',
  Low = 'Low'
}

export interface SystemMaintenance {
  maintenanceId: string;
  maintenanceType: MaintenanceType;
  description: string;
  scheduledStartTime: Date;
  estimatedDuration: number;
  actualStartTime?: Date;
  actualDuration?: number;
  completedTime?: Date;
  affectedServices: string[];
  status: MaintenanceStatus;
}

export interface SystemBackup {
  backupId: string;
  backupType: BackupType;
  startedAt: Date;
  completedAt?: Date;
  backupSize?: number;
  backupLocation?: string;
  status: BackupStatus;
  failureReason?: string;
}

export interface SystemError {
  errorId: string;
  errorType: string;
  errorMessage: string;
  stackTrace?: string;
  severity: ErrorSeverity;
  affectedAccounts?: string[];
  occurredAt: Date;
  resolvedAt?: Date;
  resolvedBy?: string;
}

export interface SystemHealth {
  status: 'Healthy' | 'Degraded' | 'Unhealthy';
  uptime: number;
  lastCheck: Date;
  services: ServiceHealth[];
}

export interface ServiceHealth {
  serviceName: string;
  status: 'Healthy' | 'Degraded' | 'Unhealthy';
  responseTime: number;
  lastCheck: Date;
}

export interface SystemMetrics {
  requestsPerSecond: number;
  responseTimeP50: number;
  responseTimeP95: number;
  responseTimeP99: number;
  errorRate: number;
  activeSessions: number;
  cpuUsage: number;
  memoryUsage: number;
  diskUsage: number;
}

export interface ScheduleMaintenanceRequest {
  maintenanceType: MaintenanceType;
  description: string;
  scheduledStartTime: Date;
  estimatedDurationMinutes: number;
  affectedServices: string[];
}

export interface ResolveErrorRequest {
  resolution: string;
}
