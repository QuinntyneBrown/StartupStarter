# System Events

## System.Maintenance.Scheduled
**Description**: Fired when system maintenance is scheduled

**Payload**:
- MaintenanceId: string
- ScheduledStartTime: DateTime
- EstimatedDuration: TimeSpan
- MaintenanceType: enum (Planned, Emergency)
- AffectedServices: List<string>
- Timestamp: DateTime

---

## System.Maintenance.Started
**Description**: Fired when system maintenance begins

**Payload**:
- MaintenanceId: string
- StartTime: DateTime
- Timestamp: DateTime

---

## System.Maintenance.Completed
**Description**: Fired when system maintenance is completed

**Payload**:
- MaintenanceId: string
- CompletedTime: DateTime
- ActualDuration: TimeSpan
- Timestamp: DateTime

---

## System.Backup.Started
**Description**: Fired when a system backup begins

**Payload**:
- BackupId: string
- BackupType: enum (Full, Incremental, Differential)
- Timestamp: DateTime

---

## System.Backup.Completed
**Description**: Fired when a system backup completes

**Payload**:
- BackupId: string
- BackupSize: long
- Duration: TimeSpan
- BackupLocation: string
- Timestamp: DateTime

---

## System.Backup.Failed
**Description**: Fired when a system backup fails

**Payload**:
- BackupId: string
- FailureReason: string
- Timestamp: DateTime

---

## System.Error.Occurred
**Description**: Fired when a system-level error occurs

**Payload**:
- ErrorId: string
- ErrorType: string
- ErrorMessage: string
- StackTrace: string
- Severity: enum (Low, Medium, High, Critical)
- AffectedAccounts: int
- Timestamp: DateTime

---

## System.Performance.Threshold.Exceeded
**Description**: Fired when system performance metrics exceed thresholds

**Payload**:
- MetricName: string
- ThresholdValue: double
- ActualValue: double
- Duration: TimeSpan
- Timestamp: DateTime
