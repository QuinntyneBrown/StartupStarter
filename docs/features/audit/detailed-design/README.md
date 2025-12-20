# Audit - Detailed Design

## Overview
Comprehensive audit logging for compliance and security monitoring.

## Aggregates
- **AuditLog**: Immutable audit records
- **AuditExport**: Export request handling
- **RetentionPolicy**: Data retention rules

## Dependencies
- All aggregates publish to audit
- Storage service for exports

## Business Rules
1. Audit logs are immutable
2. Retention: 7 years for financial, 90 days for operational
3. Exports in CSV, JSON, PDF formats
4. Automatic cleanup based on retention policy

## Data Model
- AuditLogs: EntityType, EntityId, Action, Before/After state, Timestamp
- Partitioned by date for performance
- Indexed on AccountId, EntityType, Timestamp

## API Endpoints
- GET /api/audit/logs - Query audit logs
- POST /api/audit/export - Request export
- GET /api/audit/export/{id} - Download export

## Performance
- Write-optimized append-only log
- Async event handlers
- Batch inserts
- Archival to cold storage after 1 year
