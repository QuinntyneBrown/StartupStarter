# Audit Events

## Audit.Log.Created
**Description**: Fired when an audit log entry is created for compliance

**Payload**:
- AuditId: string
- EntityType: string
- EntityId: string
- AccountId: string (if applicable)
- Action: string
- PerformedBy: string (UserId or System)
- IPAddress: string
- BeforeState: object
- AfterState: object
- Timestamp: DateTime

---

## Audit.Export.Requested
**Description**: Fired when audit logs are requested for export

**Payload**:
- ExportId: string
- AccountId: string (if account-specific export)
- RequestedBy: string (AdminId or UserId)
- DateRange: object { StartDate: DateTime, EndDate: DateTime }
- Filters: Dictionary<string, object>
- Timestamp: DateTime

---

## Audit.Export.Completed
**Description**: Fired when audit log export is completed

**Payload**:
- ExportId: string
- AccountId: string (if applicable)
- FileFormat: enum (CSV, JSON, PDF)
- RecordCount: int
- FileLocation: string
- Timestamp: DateTime

---

## Audit.Retention.PolicyApplied
**Description**: Fired when audit retention policy removes old logs

**Payload**:
- RetentionPolicyId: string
- DeletedRecordCount: int
- OldestDateRetained: DateTime
- Timestamp: DateTime
