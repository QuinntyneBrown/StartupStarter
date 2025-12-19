# Audit Domain Models

This document defines the C# models needed to implement the audit management events following Clean Architecture principles with MediatR.

## Domain Entities

### AuditLog (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class AuditLog : AggregateRoot
    {
        public Guid AuditId { get; private set; }
        public string EntityType { get; private set; }
        public string EntityId { get; private set; }
        public Guid? AccountId { get; private set; }
        public string Action { get; private set; }
        public string PerformedBy { get; private set; }
        public string IPAddress { get; private set; }
        public string? BeforeState { get; private set; }
        public string? AfterState { get; private set; }
        public DateTime Timestamp { get; private set; }
        public AuditSeverity Severity { get; private set; }
        public string? UserAgent { get; private set; }
        public string? SessionId { get; private set; }
        public bool IsCompliant { get; private set; }
        public DateTime? RetentionExpiryDate { get; private set; }

        // Navigation properties
        public virtual Account? Account { get; private set; }

        private AuditLog() { } // EF Core

        public static AuditLog Create(
            string entityType,
            string entityId,
            Guid? accountId,
            string action,
            string performedBy,
            string ipAddress,
            object? beforeState = null,
            object? afterState = null,
            AuditSeverity severity = AuditSeverity.Information,
            string? userAgent = null,
            string? sessionId = null)
        {
            var auditLog = new AuditLog
            {
                AuditId = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                AccountId = accountId,
                Action = action,
                PerformedBy = performedBy,
                IPAddress = ipAddress,
                BeforeState = beforeState != null ? JsonSerializer.Serialize(beforeState) : null,
                AfterState = afterState != null ? JsonSerializer.Serialize(afterState) : null,
                Timestamp = DateTime.UtcNow,
                Severity = severity,
                UserAgent = userAgent,
                SessionId = sessionId,
                IsCompliant = true
            };

            auditLog.AddDomainEvent(new AuditLogCreatedDomainEvent(
                auditLog.AuditId,
                auditLog.EntityType,
                auditLog.EntityId,
                auditLog.AccountId,
                auditLog.Action,
                auditLog.PerformedBy,
                auditLog.IPAddress,
                beforeState,
                afterState,
                auditLog.Timestamp));

            return auditLog;
        }

        public void SetRetentionExpiry(DateTime expiryDate)
        {
            RetentionExpiryDate = expiryDate;
        }

        public void MarkAsNonCompliant()
        {
            IsCompliant = false;
        }

        public bool IsExpired()
        {
            return RetentionExpiryDate.HasValue && DateTime.UtcNow >= RetentionExpiryDate.Value;
        }
    }
}
```

### AuditExport (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class AuditExport
    {
        public Guid ExportId { get; private set; }
        public Guid? AccountId { get; private set; }
        public Guid RequestedBy { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string? FilterCriteria { get; private set; }
        public ExportFormat FileFormat { get; private set; }
        public ExportStatus Status { get; private set; }
        public int? RecordCount { get; private set; }
        public string? FileLocation { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime RequestedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }

        // Navigation
        public virtual User Requester { get; private set; }
        public virtual Account? Account { get; private set; }

        private AuditExport() { } // EF Core

        public static AuditExport Create(
            Guid? accountId,
            Guid requestedBy,
            DateTime startDate,
            DateTime endDate,
            Dictionary<string, object>? filters = null,
            ExportFormat fileFormat = ExportFormat.CSV)
        {
            var export = new AuditExport
            {
                ExportId = Guid.NewGuid(),
                AccountId = accountId,
                RequestedBy = requestedBy,
                StartDate = startDate,
                EndDate = endDate,
                FilterCriteria = filters != null ? JsonSerializer.Serialize(filters) : null,
                FileFormat = fileFormat,
                Status = ExportStatus.Requested,
                RequestedAt = DateTime.UtcNow
            };

            return export;
        }

        public void MarkAsProcessing()
        {
            Status = ExportStatus.Processing;
        }

        public void Complete(int recordCount, string fileLocation)
        {
            Status = ExportStatus.Completed;
            RecordCount = recordCount;
            FileLocation = fileLocation;
            CompletedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddDays(30); // Exports expire after 30 days
        }

        public void Fail(string errorMessage)
        {
            Status = ExportStatus.Failed;
            ErrorMessage = errorMessage;
            CompletedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
        }
    }
}
```

### AuditRetentionPolicy (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class AuditRetentionPolicy
    {
        public Guid PolicyId { get; private set; }
        public Guid? AccountId { get; private set; }
        public string EntityType { get; private set; }
        public int RetentionDays { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation
        public virtual Account? Account { get; private set; }

        private AuditRetentionPolicy() { } // EF Core

        public static AuditRetentionPolicy Create(
            Guid? accountId,
            string entityType,
            int retentionDays)
        {
            if (retentionDays < 1)
                throw new ArgumentException("Retention days must be at least 1", nameof(retentionDays));

            return new AuditRetentionPolicy
            {
                PolicyId = Guid.NewGuid(),
                AccountId = accountId,
                EntityType = entityType,
                RetentionDays = retentionDays,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateRetentionDays(int newRetentionDays)
        {
            if (newRetentionDays < 1)
                throw new ArgumentException("Retention days must be at least 1", nameof(newRetentionDays));

            RetentionDays = newRetentionDays;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
```

## Enumerations

### AuditSeverity

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum AuditSeverity
    {
        Verbose = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        Critical = 4,
        SecurityCritical = 5
    }
}
```

### ExportFormat

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ExportFormat
    {
        CSV = 0,
        JSON = 1,
        PDF = 2,
        Excel = 3
    }
}
```

### ExportStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ExportStatus
    {
        Requested = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Expired = 4
    }
}
```

## Domain Events

### AuditLogCreatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record AuditLogCreatedDomainEvent(
        Guid AuditId,
        string EntityType,
        string EntityId,
        Guid? AccountId,
        string Action,
        string PerformedBy,
        string IPAddress,
        object? BeforeState,
        object? AfterState,
        DateTime Timestamp) : DomainEvent;
}
```

### AuditExportRequestedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record AuditExportRequestedDomainEvent(
        Guid ExportId,
        Guid? AccountId,
        Guid RequestedBy,
        DateTime StartDate,
        DateTime EndDate,
        Dictionary<string, object>? Filters,
        DateTime Timestamp) : DomainEvent;
}
```

### AuditExportCompletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record AuditExportCompletedDomainEvent(
        Guid ExportId,
        Guid? AccountId,
        ExportFormat FileFormat,
        int RecordCount,
        string FileLocation,
        DateTime Timestamp) : DomainEvent;
}
```

### AuditRetentionPolicyAppliedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record AuditRetentionPolicyAppliedDomainEvent(
        Guid RetentionPolicyId,
        int DeletedRecordCount,
        DateTime OldestDateRetained,
        DateTime Timestamp) : DomainEvent;
}
```

## MediatR Commands

### CreateAuditLogCommand

```csharp
namespace StartupStarter.Application.Audit.Commands
{
    public record CreateAuditLogCommand(
        string EntityType,
        string EntityId,
        Guid? AccountId,
        string Action,
        string PerformedBy,
        string IPAddress,
        object? BeforeState = null,
        object? AfterState = null,
        AuditSeverity Severity = AuditSeverity.Information,
        string? UserAgent = null,
        string? SessionId = null) : IRequest<CreateAuditLogResponse>;

    public record CreateAuditLogResponse(
        Guid AuditId,
        DateTime Timestamp);
}
```

### RequestAuditExportCommand

```csharp
namespace StartupStarter.Application.Audit.Commands
{
    public record RequestAuditExportCommand(
        Guid? AccountId,
        Guid RequestedBy,
        DateTime StartDate,
        DateTime EndDate,
        Dictionary<string, object>? Filters = null,
        ExportFormat FileFormat = ExportFormat.CSV) : IRequest<RequestAuditExportResponse>;

    public record RequestAuditExportResponse(
        Guid ExportId,
        ExportStatus Status);
}
```

### ProcessAuditExportCommand

```csharp
namespace StartupStarter.Application.Audit.Commands
{
    public record ProcessAuditExportCommand(
        Guid ExportId) : IRequest<Unit>;
}
```

### ApplyRetentionPolicyCommand

```csharp
namespace StartupStarter.Application.Audit.Commands
{
    public record ApplyRetentionPolicyCommand(
        Guid? AccountId = null) : IRequest<ApplyRetentionPolicyResponse>;

    public record ApplyRetentionPolicyResponse(
        int DeletedRecordCount,
        DateTime OldestDateRetained);
}
```

### CreateRetentionPolicyCommand

```csharp
namespace StartupStarter.Application.Audit.Commands
{
    public record CreateRetentionPolicyCommand(
        Guid? AccountId,
        string EntityType,
        int RetentionDays) : IRequest<CreateRetentionPolicyResponse>;

    public record CreateRetentionPolicyResponse(
        Guid PolicyId,
        bool IsActive);
}
```

### UpdateRetentionPolicyCommand

```csharp
namespace StartupStarter.Application.Audit.Commands
{
    public record UpdateRetentionPolicyCommand(
        Guid PolicyId,
        int RetentionDays) : IRequest<Unit>;
}
```

## MediatR Queries

### GetAuditLogByIdQuery

```csharp
namespace StartupStarter.Application.Audit.Queries
{
    public record GetAuditLogByIdQuery(Guid AuditId) : IRequest<AuditLogDto?>;
}
```

### GetAuditLogsByEntityQuery

```csharp
namespace StartupStarter.Application.Audit.Queries
{
    public record GetAuditLogsByEntityQuery(
        string EntityType,
        string EntityId,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<AuditLogDto>>;
}
```

### GetAuditLogsByAccountQuery

```csharp
namespace StartupStarter.Application.Audit.Queries
{
    public record GetAuditLogsByAccountQuery(
        Guid AccountId,
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        string? Action = null,
        AuditSeverity? Severity = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<AuditLogDto>>;
}
```

### GetAuditExportByIdQuery

```csharp
namespace StartupStarter.Application.Audit.Queries
{
    public record GetAuditExportByIdQuery(Guid ExportId) : IRequest<AuditExportDto?>;
}
```

### GetAuditExportsByAccountQuery

```csharp
namespace StartupStarter.Application.Audit.Queries
{
    public record GetAuditExportsByAccountQuery(
        Guid? AccountId,
        Guid? RequestedBy = null,
        ExportStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<AuditExportDto>>;
}
```

### GetRetentionPoliciesQuery

```csharp
namespace StartupStarter.Application.Audit.Queries
{
    public record GetRetentionPoliciesQuery(
        Guid? AccountId = null,
        bool? IsActive = true) : IRequest<List<AuditRetentionPolicyDto>>;
}
```

## DTOs

### AuditLogDto

```csharp
namespace StartupStarter.Application.Audit.DTOs
{
    public record AuditLogDto(
        Guid AuditId,
        string EntityType,
        string EntityId,
        Guid? AccountId,
        string AccountName,
        string Action,
        string PerformedBy,
        string PerformedByName,
        string IPAddress,
        string? BeforeState,
        string? AfterState,
        DateTime Timestamp,
        AuditSeverity Severity,
        string? UserAgent,
        string? SessionId,
        bool IsCompliant);
}
```

### AuditExportDto

```csharp
namespace StartupStarter.Application.Audit.DTOs
{
    public record AuditExportDto(
        Guid ExportId,
        Guid? AccountId,
        string AccountName,
        Guid RequestedBy,
        string RequesterName,
        DateTime StartDate,
        DateTime EndDate,
        string? FilterCriteria,
        ExportFormat FileFormat,
        ExportStatus Status,
        int? RecordCount,
        string? FileLocation,
        string? ErrorMessage,
        DateTime RequestedAt,
        DateTime? CompletedAt,
        DateTime? ExpiresAt);
}
```

### AuditRetentionPolicyDto

```csharp
namespace StartupStarter.Application.Audit.DTOs
{
    public record AuditRetentionPolicyDto(
        Guid PolicyId,
        Guid? AccountId,
        string AccountName,
        string EntityType,
        int RetentionDays,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
```

### AuditSummaryDto

```csharp
namespace StartupStarter.Application.Audit.DTOs
{
    public record AuditSummaryDto(
        int TotalLogs,
        int LogsLast24Hours,
        int LogsLast7Days,
        int LogsLast30Days,
        Dictionary<string, int> ActionCounts,
        Dictionary<AuditSeverity, int> SeverityCounts,
        Dictionary<string, int> EntityTypeCounts);
}
```

## Base Classes

### AggregateRoot

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
```

### DomainEvent

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
```

## Repository Interfaces

### IAuditLogRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog?> GetByIdAsync(Guid auditId, CancellationToken cancellationToken = default);
        Task<List<AuditLog>> GetByEntityAsync(
            string entityType,
            string entityId,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<AuditLog>> GetPagedByEntityAsync(
            string entityType,
            string entityId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<AuditLog>> GetPagedByAccountAsync(
            Guid accountId,
            DateTime? startDate,
            DateTime? endDate,
            string? action,
            AuditSeverity? severity,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<AuditLog>> GetForExportAsync(
            Guid? accountId,
            DateTime startDate,
            DateTime endDate,
            Dictionary<string, object>? filters = null,
            CancellationToken cancellationToken = default);
        Task<int> DeleteExpiredLogsAsync(
            DateTime cutoffDate,
            Guid? accountId = null,
            CancellationToken cancellationToken = default);
        Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IAuditExportRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IAuditExportRepository
    {
        Task<AuditExport?> GetByIdAsync(Guid exportId, CancellationToken cancellationToken = default);
        Task<PaginatedList<AuditExport>> GetPagedByAccountAsync(
            Guid? accountId,
            Guid? requestedBy,
            ExportStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<AuditExport>> GetExpiredExportsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(AuditExport export, CancellationToken cancellationToken = default);
        Task UpdateAsync(AuditExport export, CancellationToken cancellationToken = default);
        Task DeleteAsync(AuditExport export, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IAuditRetentionPolicyRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IAuditRetentionPolicyRepository
    {
        Task<AuditRetentionPolicy?> GetByIdAsync(Guid policyId, CancellationToken cancellationToken = default);
        Task<List<AuditRetentionPolicy>> GetActiveByAccountAsync(
            Guid? accountId,
            CancellationToken cancellationToken = default);
        Task<AuditRetentionPolicy?> GetByEntityTypeAsync(
            Guid? accountId,
            string entityType,
            CancellationToken cancellationToken = default);
        Task<List<AuditRetentionPolicy>> GetAllActiveAsync(CancellationToken cancellationToken = default);
        Task AddAsync(AuditRetentionPolicy policy, CancellationToken cancellationToken = default);
        Task UpdateAsync(AuditRetentionPolicy policy, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

## Application Services

### IAuditExportService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IAuditExportService
    {
        Task<string> ExportToCSVAsync(
            List<AuditLog> logs,
            CancellationToken cancellationToken = default);
        Task<string> ExportToJSONAsync(
            List<AuditLog> logs,
            CancellationToken cancellationToken = default);
        Task<string> ExportToPDFAsync(
            List<AuditLog> logs,
            CancellationToken cancellationToken = default);
        Task<string> ExportToExcelAsync(
            List<AuditLog> logs,
            CancellationToken cancellationToken = default);
        Task<string> UploadToStorageAsync(
            string filePath,
            string fileName,
            CancellationToken cancellationToken = default);
    }
}
```

### IAuditComplianceService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IAuditComplianceService
    {
        Task<bool> ValidateGDPRComplianceAsync(AuditLog auditLog);
        Task<bool> ValidateSOC2ComplianceAsync(AuditLog auditLog);
        Task<bool> ValidateHIPAAComplianceAsync(AuditLog auditLog);
        Task<ComplianceReport> GenerateComplianceReportAsync(
            Guid? accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
    }
}
```
