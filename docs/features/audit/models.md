# Audit Models

## Core Aggregate

### AuditAggregate

Located in: `StartupStarter.Core\Model\AuditAggregate\`

#### Entities

**AuditLog.cs** (Aggregate Root)
```csharp
public class AuditLog
{
    public string AuditId { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string AccountId { get; private set; }
    public string Action { get; private set; }
    public string PerformedBy { get; private set; }
    public string IPAddress { get; private set; }
    public string BeforeStateJson { get; private set; }
    public string AfterStateJson { get; private set; }
    public DateTime Timestamp { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static AuditLog Create(string entityType, string entityId, string accountId, string action,
        string performedBy, string ipAddress, object beforeState, object afterState);
}
```

**AuditExport.cs**
```csharp
public class AuditExport
{
    public string ExportId { get; private set; }
    public string AccountId { get; private set; }
    public string RequestedBy { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string FiltersJson { get; private set; }
    public FileFormat FileFormat { get; private set; }
    public ExportStatus Status { get; private set; }
    public int RecordCount { get; private set; }
    public string FileLocation { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void MarkCompleted(int recordCount, string fileLocation);
}
```

**RetentionPolicy.cs**
```csharp
public class RetentionPolicy
{
    public string RetentionPolicyId { get; private set; }
    public string PolicyName { get; private set; }
    public int RetentionDays { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Apply(int deletedRecordCount, DateTime oldestDateRetained);
}
```

#### Enums

**FileFormat.cs**
```csharp
public enum FileFormat
{
    CSV,
    JSON,
    PDF
}
```

**ExportStatus.cs**
```csharp
public enum ExportStatus
{
    Requested,
    InProgress,
    Completed,
    Failed
}
```

#### Domain Events

**AuditLogCreatedEvent.cs**
```csharp
public class AuditLogCreatedEvent : DomainEvent
{
    public string AuditId { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string AccountId { get; set; }
    public string Action { get; set; }
    public string PerformedBy { get; set; }
    public string IPAddress { get; set; }
    public object BeforeState { get; set; }
    public object AfterState { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AuditExportRequestedEvent.cs**
```csharp
public class AuditExportRequestedEvent : DomainEvent
{
    public string ExportId { get; set; }
    public string AccountId { get; set; }
    public string RequestedBy { get; set; }
    public object DateRange { get; set; }
    public Dictionary<string, object> Filters { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AuditExportCompletedEvent.cs**
```csharp
public class AuditExportCompletedEvent : DomainEvent
{
    public string ExportId { get; set; }
    public string AccountId { get; set; }
    public FileFormat FileFormat { get; set; }
    public int RecordCount { get; set; }
    public string FileLocation { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AuditRetentionPolicyAppliedEvent.cs**
```csharp
public class AuditRetentionPolicyAppliedEvent : DomainEvent
{
    public string RetentionPolicyId { get; set; }
    public int DeletedRecordCount { get; set; }
    public DateTime OldestDateRetained { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Infrastructure

### Entity Configuration

**AuditLogConfiguration.cs**
```csharp
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(a => a.AuditId);

        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.IPAddress).HasMaxLength(45);

        builder.HasIndex(a => new { a.AccountId, a.Timestamp });
        builder.HasIndex(a => new { a.EntityType, a.EntityId });

        builder.Ignore(a => a.DomainEvents);
    }
}
```

**AuditExportConfiguration.cs**
```csharp
public class AuditExportConfiguration : IEntityTypeConfiguration<AuditExport>
{
    public void Configure(EntityTypeBuilder<AuditExport> builder)
    {
        builder.ToTable("AuditExports");
        builder.HasKey(e => e.ExportId);

        builder.Property(e => e.FileLocation).HasMaxLength(500);

        builder.Ignore(e => e.DomainEvents);
    }
}
```

## API Layer

### DTOs

**AuditLogDto.cs**
```csharp
public class AuditLogDto
{
    public string AuditId { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string AccountId { get; set; }
    public string Action { get; set; }
    public string PerformedBy { get; set; }
    public string IPAddress { get; set; }
    public object BeforeState { get; set; }
    public object AfterState { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**AuditExportDto.cs**
```csharp
public class AuditExportDto
{
    public string ExportId { get; set; }
    public string AccountId { get; set; }
    public string FileFormat { get; set; }
    public string Status { get; set; }
    public int RecordCount { get; set; }
    public string FileLocation { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
```

### Extension Methods

**AuditExtensions.cs**
```csharp
public static class AuditExtensions
{
    public static AuditLogDto ToDto(this AuditLog audit)
    {
        return new AuditLogDto
        {
            AuditId = audit.AuditId,
            EntityType = audit.EntityType,
            EntityId = audit.EntityId,
            AccountId = audit.AccountId,
            Action = audit.Action,
            PerformedBy = audit.PerformedBy,
            IPAddress = audit.IPAddress,
            BeforeState = JsonSerializer.Deserialize<object>(audit.BeforeStateJson),
            AfterState = JsonSerializer.Deserialize<object>(audit.AfterStateJson),
            Timestamp = audit.Timestamp
        };
    }
}
```

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\Audit\Commands\`

**CreateAuditLogCommand.cs**
```csharp
public class CreateAuditLogCommand : IRequest<bool>
{
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string AccountId { get; set; }
    public string Action { get; set; }
    public string PerformedBy { get; set; }
    public string IPAddress { get; set; }
    public object BeforeState { get; set; }
    public object AfterState { get; set; }
}
```

**RequestAuditExportCommand.cs**
```csharp
public class RequestAuditExportCommand : IRequest<AuditExportDto>
{
    public string AccountId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, object> Filters { get; set; }
    public FileFormat FileFormat { get; set; }
    public string RequestedBy { get; set; }
}
```

### Queries (MediatR)

Located in: `StartupStarter.Api\Features\Audit\Queries\`

**GetAuditLogsByAccountQuery.cs**
```csharp
public class GetAuditLogsByAccountQuery : IRequest<List<AuditLogDto>>
{
    public string AccountId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
```

**GetAuditLogsByEntityQuery.cs**
```csharp
public class GetAuditLogsByEntityQuery : IRequest<List<AuditLogDto>>
{
    public string EntityType { get; set; }
    public string EntityId { get; set; }
}
```
