# Audit Management - Detailed Design

This folder contains the detailed design documentation for the Audit Management feature implementation using Clean Architecture, MediatR, and Angular 21.

## Overview

The Audit Management system provides comprehensive audit logging, compliance reporting, and data retention capabilities. The implementation follows Clean Architecture principles with clear separation of concerns and strong emphasis on compliance with GDPR, SOC2, and HIPAA regulations.

## Architecture Documents

### 1. Domain Model (`domain-model.puml`)

**PlantUML class diagram** showing:
- **AuditLog** aggregate root - immutable audit records
- **AuditExport** entity for compliance report generation
- **AuditRetentionPolicy** entity for automated data lifecycle management
- Domain events raised during audit operations
- Value objects (AuditSeverity, ExportFormat, ExportStatus)
- Relationships between entities

**Key Design Patterns:**
- Aggregate Root pattern (DDD)
- Domain Events pattern
- Immutable audit records
- Value Objects for type safety

### 2. Sequence Diagrams (`sequence-diagrams.puml`)

**Comprehensive flow diagrams** for:
- **Create Audit Log**: Automatic capture via AOP interceptor with compliance validation
- **Request Export**: Compliance officer initiates audit log export with rate limiting
- **Process Export**: Background job generates export file in multiple formats
- **Apply Retention Policy**: Automated cleanup based on compliance requirements

**Technology Integration:**
- MediatR request/response pipeline
- FluentValidation in pipeline behaviors
- Entity Framework Core with DbContext
- Azure Service Bus for integration events
- SignalR for real-time notifications
- Hangfire/Azure Functions for background jobs
- Azure Blob Storage for export files
- SIEM integration for security monitoring

### 3. Component Diagram (`component-diagram.puml`)

**PlantUML component architecture** showing:
- Component relationships across all layers
- Dependency flow from UI to database
- MediatR pipeline execution
- Event dispatching mechanism
- Integration with external Azure services
- AOP interceptor for automatic audit capture
- Compliance validation services

## Technology Stack

### Backend
- **.NET 8** - Web API framework
- **MediatR** - CQRS and mediator pattern implementation
- **Entity Framework Core** - ORM and data access
- **FluentValidation** - Request validation
- **AutoMapper** - Object-to-object mapping
- **SignalR** - Real-time communication
- **Hangfire/Azure Functions** - Background job processing
- **AspectCore/Castle.DynamicProxy** - AOP for audit interception

### Frontend
- **Angular 21** - Admin UI framework
- **RxJS** - Reactive state management
- **Angular Material** - UI components
- **Chart.js** - Audit analytics visualization
- **TypeScript** - Type-safe development

### Azure Services
- **Azure SQL Database** - Audit log storage with partitioning
- **Azure Service Bus** - Message broker for integration events
- **Azure Blob Storage** - Export file storage with SAS tokens
- **Azure Monitor** - Performance monitoring
- **Application Insights** - Telemetry and logging
- **Azure Key Vault** - Secrets management

## Implementation Guidelines

### Clean Architecture Principles

1. **Dependency Rule**: Dependencies point inward
   - Presentation → Application → Domain ← Infrastructure
   - Domain has no dependencies on other layers
   - Infrastructure implements interfaces defined in Domain/Application

2. **Separation of Concerns**
   - **Domain**: Audit business logic and compliance rules
   - **Application**: Use cases and orchestration
   - **Infrastructure**: External concerns (database, storage, messaging)
   - **Presentation**: User interface and API endpoints

3. **Testability**
   - Domain logic is pure and easily testable
   - Application handlers can be unit tested with mocked repositories
   - Integration tests for Infrastructure layer

### MediatR Pattern

```csharp
// Command
public record CreateAuditLogCommand(
    string EntityType,
    string EntityId,
    string Action,
    string PerformedBy,
    string IPAddress) : IRequest<CreateAuditLogResponse>;

// Handler
public class CreateAuditLogHandler : IRequestHandler<CreateAuditLogCommand, CreateAuditLogResponse>
{
    private readonly IAuditLogRepository _repository;
    private readonly IComplianceService _complianceService;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<CreateAuditLogResponse> Handle(
        CreateAuditLogCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create domain entity (raises domain events)
        var auditLog = AuditLog.Create(
            request.EntityType,
            request.EntityId,
            request.Action,
            request.PerformedBy,
            request.IPAddress);

        // 2. Validate compliance
        await _complianceService.ValidateComplianceAsync(auditLog);

        // 3. Persist to database
        await _repository.AddAsync(auditLog, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 4. Dispatch domain events (after save)
        await _eventDispatcher.DispatchAsync(auditLog.DomainEvents, cancellationToken);

        // 5. Return response
        return new CreateAuditLogResponse(auditLog.AuditId, auditLog.Timestamp);
    }
}
```

### Domain-Driven Design

**Aggregate Root (AuditLog):**
- Enforces business invariants
- Immutable once created (no updates allowed)
- Raises domain events for state changes
- Provides factory methods (e.g., `AuditLog.Create()`)
- Encapsulates all business logic

**Domain Events:**
- Raised by aggregate roots
- Represent state changes
- Dispatched after successful persistence
- Enable event-driven architecture

### Aspect-Oriented Programming (AOP)

**Audit Interceptor:**
```csharp
public class AuditInterceptor : IInterceptor
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task Intercept(IInvocation invocation)
    {
        // Capture before state
        var beforeState = CaptureState(invocation);

        // Execute method
        await invocation.Proceed();

        // Capture after state
        var afterState = CaptureState(invocation);

        // Create audit log asynchronously
        _ = Task.Run(() => _mediator.Send(new CreateAuditLogCommand(
            entityType: invocation.TargetType.Name,
            entityId: ExtractEntityId(invocation),
            action: invocation.Method.Name,
            performedBy: _httpContextAccessor.HttpContext.User.GetUserId(),
            ipAddress: _httpContextAccessor.HttpContext.Connection.RemoteIpAddress,
            beforeState: beforeState,
            afterState: afterState)));
    }
}
```

## Compliance Considerations

### GDPR (General Data Protection Regulation)

**Requirements:**
- Right to Access: Users can request their complete audit history
- Right to Erasure: Ability to delete audit logs (with limitations for legal compliance)
- Data Portability: Export audit logs in machine-readable formats (JSON, CSV)
- Audit Trail: All data access and modifications must be logged
- Retention Limits: Personal data should not be kept longer than necessary

**Implementation:**
```csharp
// GDPR-compliant export
public class GDPRExportHandler
{
    public async Task<AuditExportDto> ExportUserData(Guid userId)
    {
        var auditLogs = await _auditRepo.GetByUserAsync(userId);

        // Include all personal data access
        var export = await _exportService.ExportToJSONAsync(auditLogs);

        // Ensure export is encrypted and time-limited
        return new AuditExportDto
        {
            FileLocation = await _blobStorage.UploadWithSASToken(export, expiryHours: 24),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}

// Right to be forgotten
public class EraseUserDataHandler
{
    public async Task EraseUserAuditData(Guid userId, string legalBasis)
    {
        // Validate legal basis for deletion
        if (!IsValidLegalBasis(legalBasis))
            throw new ComplianceException("Invalid legal basis for data erasure");

        // Anonymize instead of delete (preserve audit integrity)
        await _auditRepo.AnonymizeUserDataAsync(userId);

        // Log the erasure itself
        await _auditRepo.AddAsync(AuditLog.Create(
            "User", userId.ToString(), "GDPR_Erasure", "System", "Internal",
            severity: AuditSeverity.Critical));
    }
}
```

**Retention Policy:**
- Default: 90 days for regular operations
- Extended: 6-7 years for financial/legal compliance
- Minimum: 30 days for security-related events

### SOC2 (Service Organization Control 2)

**Requirements:**
- Security: Track all authentication and authorization events
- Availability: Log system availability and uptime
- Processing Integrity: Audit all data processing operations
- Confidentiality: Track access to confidential data
- Privacy: Monitor privacy-related data access

**Implementation:**
```csharp
// SOC2 Trust Service Criteria mapping
public enum SOC2Category
{
    Security,           // Authentication, authorization, access control
    Availability,       // System uptime, disaster recovery
    ProcessingIntegrity, // Data processing, validation, errors
    Confidentiality,    // Sensitive data access
    Privacy             // Personal data handling
}

// Automatic categorization
public class SOC2ComplianceService
{
    public SOC2Category CategorizeAuditLog(AuditLog log)
    {
        return log.Action switch
        {
            "Login" or "Logout" or "PermissionChanged" => SOC2Category.Security,
            "SystemDown" or "SystemRestart" => SOC2Category.Availability,
            "DataValidation" or "ProcessingError" => SOC2Category.ProcessingIntegrity,
            "ConfidentialDataAccess" => SOC2Category.Confidentiality,
            "PersonalDataAccess" or "ConsentChanged" => SOC2Category.Privacy,
            _ => SOC2Category.ProcessingIntegrity
        };
    }

    public async Task<ComplianceReport> GenerateSOC2Report(DateTime startDate, DateTime endDate)
    {
        var auditLogs = await _auditRepo.GetByDateRangeAsync(startDate, endDate);

        return new ComplianceReport
        {
            TotalEvents = auditLogs.Count,
            SecurityEvents = auditLogs.Count(l => CategorizeAuditLog(l) == SOC2Category.Security),
            FailedLogins = auditLogs.Count(l => l.Action == "FailedLogin"),
            UnauthorizedAccess = auditLogs.Count(l => l.Action == "UnauthorizedAccess"),
            DataBreaches = auditLogs.Count(l => l.Severity == AuditSeverity.SecurityCritical),
            ComplianceScore = CalculateComplianceScore(auditLogs)
        };
    }
}
```

**Retention Policy:**
- Minimum: 90 days for all events
- Security-critical events: 1 year minimum
- Access to sensitive data: Match data retention policy

### HIPAA (Health Insurance Portability and Accountability Act)

**Requirements:**
- Access Logging: All PHI (Protected Health Information) access must be logged
- Audit Controls: Technical security measures to record and examine PHI access
- Retention: Minimum 6 years for all audit logs
- Encryption: Audit logs containing PHI must be encrypted
- Breach Notification: Unauthorized PHI access must be tracked and reported

**Implementation:**
```csharp
// HIPAA-compliant audit logging
public class HIPAAComplianceService
{
    private readonly IAuditLogRepository _auditRepo;
    private readonly IEncryptionService _encryption;

    public async Task LogPHIAccess(
        string entityType,
        string entityId,
        Guid userId,
        string action,
        string ipAddress)
    {
        // All PHI access is security-critical
        var auditLog = AuditLog.Create(
            entityType: entityType,
            entityId: entityId,
            action: action,
            performedBy: userId.ToString(),
            ipAddress: ipAddress,
            severity: AuditSeverity.SecurityCritical);

        // Set 6-year retention for HIPAA compliance
        auditLog.SetRetentionExpiry(DateTime.UtcNow.AddYears(6));

        // Encrypt sensitive data
        if (auditLog.BeforeState != null)
            auditLog.BeforeState = await _encryption.EncryptAsync(auditLog.BeforeState);
        if (auditLog.AfterState != null)
            auditLog.AfterState = await _encryption.EncryptAsync(auditLog.AfterState);

        await _auditRepo.AddAsync(auditLog);
        await _auditRepo.SaveChangesAsync();

        // Check for potential breaches
        await DetectUnauthorizedAccess(auditLog);
    }

    private async Task DetectUnauthorizedAccess(AuditLog log)
    {
        // Pattern detection for suspicious activity
        var recentLogs = await _auditRepo.GetRecentByUserAsync(log.PerformedBy, TimeSpan.FromMinutes(5));

        // Multiple failed access attempts
        if (recentLogs.Count(l => l.Action.Contains("Unauthorized")) >= 3)
        {
            await NotifySecurityTeam(log);
            await CreateSecurityIncident(log);
        }

        // Access from unusual IP or time
        if (IsUnusualAccess(log))
        {
            await FlagForReview(log);
        }
    }
}

// HIPAA retention policy
public class HIPAARetentionPolicy : AuditRetentionPolicy
{
    public HIPAARetentionPolicy() : base(
        accountId: null,
        entityType: "Patient|MedicalRecord|Prescription|Diagnosis",
        retentionDays: 2190) // 6 years
    {
    }

    public override bool CanDelete(AuditLog log)
    {
        // Never delete security-critical PHI access logs
        if (log.Severity == AuditSeverity.SecurityCritical)
            return false;

        // Respect 6-year minimum retention
        return log.Timestamp < DateTime.UtcNow.AddYears(-6);
    }
}
```

**Breach Detection:**
```csharp
public class BreachDetectionService
{
    public async Task<bool> DetectPotentialBreach(AuditLog log)
    {
        var indicators = new List<bool>
        {
            await CheckMultipleFailedAttempts(log),
            await CheckUnusualAccessPattern(log),
            await CheckAccessFromNewLocation(log),
            await CheckBulkDataAccess(log),
            await CheckAccessOutsideBusinessHours(log)
        };

        if (indicators.Count(i => i) >= 2)
        {
            await CreateBreachAlert(log);
            return true;
        }

        return false;
    }
}
```

## Key Workflows

### Automatic Audit Capture Flow
1. User performs tracked operation (e.g., Update User)
2. AOP Interceptor captures operation before/after state
3. Interceptor extracts HTTP context (UserId, IP, UserAgent)
4. Sends `CreateAuditLogCommand` via MediatR (async, non-blocking)
5. Handler creates immutable AuditLog aggregate
6. Compliance validation (GDPR/SOC2/HIPAA)
7. Persists to database with calculated retention expiry
8. Publishes integration events to Service Bus and SIEM

### Audit Export Flow
1. Compliance officer selects date range and filters in UI
2. HTTP POST to `/api/audit/export`
3. Rate limiting check (max 10 exports per account per day)
4. Creates AuditExport entity with Status = Requested
5. Queues background job for processing
6. Returns 202 Accepted with exportId
7. Background job:
   - Retrieves audit logs matching criteria
   - Generates file in requested format (CSV/JSON/PDF/Excel)
   - Uploads to Azure Blob Storage with SAS token
   - Updates export status to Completed
   - Notifies requester via SignalR and email
8. Export expires after 30 days (automatic cleanup)

### Retention Policy Flow
1. Scheduled job runs daily at 2:00 AM UTC
2. Retrieves all active retention policies
3. For each policy:
   - Calculates cutoff date (Today - RetentionDays)
   - Validates compliance (GDPR/SOC2/HIPAA minimums)
   - Deletes/anonymizes expired audit logs
   - Raises RetentionPolicyApplied event
4. Publishes integration events for compliance reporting
5. Logs retention execution (audit of audits)

## Database Schema

### AuditLogs Table
```sql
CREATE TABLE AuditLogs (
    AuditId UNIQUEIDENTIFIER PRIMARY KEY,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId NVARCHAR(100) NOT NULL,
    AccountId UNIQUEIDENTIFIER NULL,
    Action NVARCHAR(100) NOT NULL,
    PerformedBy NVARCHAR(100) NOT NULL,
    IPAddress NVARCHAR(50) NOT NULL,
    BeforeState NVARCHAR(MAX) NULL,
    AfterState NVARCHAR(MAX) NULL,
    Timestamp DATETIME2 NOT NULL,
    Severity INT NOT NULL,
    UserAgent NVARCHAR(500) NULL,
    SessionId NVARCHAR(100) NULL,
    IsCompliant BIT NOT NULL DEFAULT 1,
    RetentionExpiryDate DATETIME2 NULL,

    INDEX IX_AuditLogs_EntityType_EntityId (EntityType, EntityId),
    INDEX IX_AuditLogs_AccountId_Timestamp (AccountId, Timestamp),
    INDEX IX_AuditLogs_PerformedBy_Timestamp (PerformedBy, Timestamp),
    INDEX IX_AuditLogs_Timestamp (Timestamp),
    INDEX IX_AuditLogs_Severity (Severity),
    INDEX IX_AuditLogs_RetentionExpiry (RetentionExpiryDate)
);

-- Partition by month for performance
ALTER TABLE AuditLogs
ADD PARTITION SCHEME AuditLogsByMonth (Timestamp);
```

### AuditExports Table
```sql
CREATE TABLE AuditExports (
    ExportId UNIQUEIDENTIFIER PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NULL,
    RequestedBy UNIQUEIDENTIFIER NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    FilterCriteria NVARCHAR(MAX) NULL,
    FileFormat INT NOT NULL,
    Status INT NOT NULL,
    RecordCount INT NULL,
    FileLocation NVARCHAR(500) NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    RequestedAt DATETIME2 NOT NULL,
    CompletedAt DATETIME2 NULL,
    ExpiresAt DATETIME2 NULL,

    FOREIGN KEY (RequestedBy) REFERENCES Users(UserId),
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),

    INDEX IX_AuditExports_AccountId_Status (AccountId, Status),
    INDEX IX_AuditExports_RequestedBy (RequestedBy),
    INDEX IX_AuditExports_ExpiresAt (ExpiresAt)
);
```

### AuditRetentionPolicies Table
```sql
CREATE TABLE AuditRetentionPolicies (
    PolicyId UNIQUEIDENTIFIER PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NULL,
    EntityType NVARCHAR(100) NOT NULL,
    RetentionDays INT NOT NULL CHECK (RetentionDays >= 1),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,

    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),

    UNIQUE (AccountId, EntityType),
    INDEX IX_AuditRetentionPolicies_IsActive (IsActive)
);
```

## Testing Strategy

### Unit Tests
- Domain entity logic (AuditLog aggregate methods)
- Command/Query handlers with mocked dependencies
- Validators using FluentValidation test helpers
- Compliance validation logic

### Integration Tests
- API endpoints with WebApplicationFactory
- Repository implementations with test database
- Event dispatching and SignalR notifications
- Background job execution
- Blob storage upload/download

### Compliance Tests
- GDPR right to access validation
- SOC2 security event categorization
- HIPAA retention policy enforcement
- Breach detection algorithms

### E2E Tests
- Complete audit capture flow
- Export generation and download
- Retention policy application
- Compliance report generation

## Security Considerations

### Authentication & Authorization
- JWT Bearer token authentication on all endpoints
- Role-based access control (AuditViewer, AuditExporter, ComplianceOfficer)
- Account-level data isolation
- IP whitelist for compliance endpoints

### Data Protection
- Encryption at rest for audit logs containing sensitive data
- TLS 1.3 for data in transit
- Azure Key Vault for encryption keys
- Secure SAS tokens for blob storage access

### Audit Integrity
- Immutable audit logs (no updates/deletes except via retention policy)
- Cryptographic hashing to detect tampering
- Separate audit trail for audit operations
- Write-once storage tier for long-term retention

### Threat Protection
- Rate limiting on export endpoints
- Anomaly detection for unusual access patterns
- SIEM integration for security monitoring
- Automated breach detection and alerting

## Performance Optimizations

### Database Optimization
- Partitioning by month for large audit tables
- Indexed foreign keys and timestamp columns
- Async operations throughout
- Read replicas for reporting queries
- Archive old logs to blob storage (reduce DB size)

### Caching Strategy
- Cache retention policies (rarely change)
- Cache user/account metadata
- Redis distributed cache for multi-instance deployments

### Background Processing
- Async audit capture (no performance impact on main operations)
- Queue-based export processing
- Scheduled batch deletion for retention

### Query Optimization
- EF Core compiled queries for frequent operations
- Pagination for all list queries
- AsNoTracking for read-only queries
- Selective field projection to reduce data transfer

## Monitoring & Logging

### Application Insights
- Request/response logging
- Exception tracking
- Custom events for compliance operations
- Performance metrics

### Structured Logging
- Serilog with JSON formatting
- Correlation IDs for request tracing
- Separate log sinks for audit vs. application logs

### Metrics
- Audit logs created per hour/day
- Export request rate and processing time
- Retention policy execution time
- Compliance score tracking
- Storage usage trends

### Alerts
- Compliance violations
- Failed export processing
- Retention policy failures
- Potential security breaches
- Unusual access patterns

## Compliance Reporting

### Automated Reports
- **Daily**: Security event summary
- **Weekly**: Compliance scorecard
- **Monthly**: Full audit report with trends
- **Quarterly**: SOC2/HIPAA compliance attestation
- **Annual**: GDPR data processing report

### Report Contents
- Total audit events by category
- Security incidents and resolutions
- Access patterns and anomalies
- Retention policy execution results
- Export requests and completions
- Compliance score over time

## Next Steps

1. **Implementation**: Follow the model definitions in `/docs/model/audit.md`
2. **Database**: Create migrations using EF Core
3. **AOP Setup**: Configure audit interceptor with DI
4. **Compliance Config**: Define retention policies per entity type
5. **Testing**: Implement unit, integration, and compliance tests
6. **Deployment**: Configure Azure resources (SQL, Blob, Service Bus)
7. **SIEM Integration**: Connect to external security monitoring
8. **Documentation**: API documentation with Swagger/OpenAPI

## References

- Event Definitions: `/docs/events/audit.md`
- Model Definitions: `/docs/model/audit.md`
- Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- MediatR Documentation: https://github.com/jbogard/MediatR
- Domain-Driven Design: Eric Evans, "Domain-Driven Design"
- GDPR Compliance: https://gdpr.eu/
- SOC2 Framework: https://www.aicpa.org/soc
- HIPAA Security Rule: https://www.hhs.gov/hipaa/for-professionals/security/

## Compliance Checklist

### GDPR Compliance
- [ ] Right to Access implemented (export user audit data)
- [ ] Right to Erasure implemented (anonymize user data)
- [ ] Data Portability supported (JSON/CSV exports)
- [ ] Consent tracking for data processing
- [ ] Breach notification within 72 hours
- [ ] Data Protection Impact Assessment (DPIA) completed
- [ ] Privacy by Design principles applied

### SOC2 Compliance
- [ ] All authentication events logged
- [ ] All authorization failures tracked
- [ ] System availability monitored
- [ ] Data processing errors logged
- [ ] Sensitive data access tracked
- [ ] 90-day minimum retention for security events
- [ ] Quarterly compliance reports generated

### HIPAA Compliance
- [ ] All PHI access logged with encryption
- [ ] 6-year retention for PHI-related logs
- [ ] Breach detection automated
- [ ] Security incident response plan
- [ ] Regular security risk assessments
- [ ] Business associate agreements in place
- [ ] HIPAA training for development team

---

**Last Updated**: December 2024
**Version**: 1.0
**Status**: Design Complete - Ready for Implementation
**Compliance Framework**: GDPR, SOC2, HIPAA
