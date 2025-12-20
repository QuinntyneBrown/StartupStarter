# System Management Models

## Core Aggregate

### MaintenanceAggregate

Located in: `StartupStarter.Core\Model\MaintenanceAggregate\`

#### Folder Structure
```
MaintenanceAggregate/
├── Entities/
│   └── SystemMaintenance.cs
├── Enums/
│   └── MaintenanceType.cs
└── Events/
    ├── SystemMaintenanceScheduledEvent.cs
    ├── SystemMaintenanceStartedEvent.cs
    └── SystemMaintenanceCompletedEvent.cs
```

**SystemMaintenance.cs** (Aggregate Root)
```csharp
public class SystemMaintenance
{
    public string MaintenanceId { get; private set; }
    public DateTime ScheduledStartTime { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    public MaintenanceType MaintenanceType { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? CompletedTime { get; private set; }
    public TimeSpan? ActualDuration { get; private set; }

    private readonly List<string> _affectedServices = new();
    public IReadOnlyCollection<string> AffectedServices => _affectedServices.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Start();
    public void Complete();
}
```

### BackupAggregate

Located in: `StartupStarter.Core\Model\BackupAggregate\`

#### Folder Structure
```
BackupAggregate/
├── Entities/
│   └── SystemBackup.cs
├── Enums/
│   └── BackupType.cs
└── Events/
    ├── SystemBackupStartedEvent.cs
    ├── SystemBackupCompletedEvent.cs
    └── SystemBackupFailedEvent.cs
```

**SystemBackup.cs** (Aggregate Root)
```csharp
public class SystemBackup
{
    public string BackupId { get; private set; }
    public BackupType BackupType { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public long? BackupSize { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public string BackupLocation { get; private set; }
    public bool Success { get; private set; }
    public string FailureReason { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Complete(long backupSize, string backupLocation);
    public void MarkFailed(string failureReason);
}
```

### SystemErrorAggregate

Located in: `StartupStarter.Core\Model\SystemErrorAggregate\`

#### Folder Structure
```
SystemErrorAggregate/
├── Entities/
│   └── SystemError.cs
├── Enums/
│   └── ErrorSeverity.cs
└── Events/
    └── SystemErrorOccurredEvent.cs
```

#### Enums

**MaintenanceType.cs**
```csharp
public enum MaintenanceType
{
    Planned,
    Emergency
}
```

**BackupType.cs**
```csharp
public enum BackupType
{
    Full,
    Incremental,
    Differential
}
```

**ErrorSeverity.cs**
```csharp
public enum ErrorSeverity
{
    Low,
    Medium,
    High,
    Critical
}
```

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\System\Commands\`

**ScheduleMaintenanceCommand.cs**, **StartBackupCommand.cs**, **RecordSystemErrorCommand.cs**
