# System Management Domain Models

This document defines the C# models needed to implement the system management events following Clean Architecture principles with MediatR.

## Domain Entities

### MaintenanceWindow (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class MaintenanceWindow : AggregateRoot
    {
        public Guid MaintenanceId { get; private set; }
        public DateTime ScheduledStartTime { get; private set; }
        public TimeSpan EstimatedDuration { get; private set; }
        public MaintenanceType MaintenanceType { get; private set; }
        public List<string> AffectedServices { get; private set; } = new();
        public DateTime? StartTime { get; private set; }
        public DateTime? CompletedTime { get; private set; }
        public TimeSpan? ActualDuration { get; private set; }
        public MaintenanceStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private MaintenanceWindow() { } // EF Core

        public static MaintenanceWindow Schedule(
            DateTime scheduledStartTime,
            TimeSpan estimatedDuration,
            MaintenanceType maintenanceType,
            List<string> affectedServices)
        {
            var maintenance = new MaintenanceWindow
            {
                MaintenanceId = Guid.NewGuid(),
                ScheduledStartTime = scheduledStartTime,
                EstimatedDuration = estimatedDuration,
                MaintenanceType = maintenanceType,
                AffectedServices = affectedServices,
                Status = MaintenanceStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            maintenance.AddDomainEvent(new MaintenanceScheduledDomainEvent(
                maintenance.MaintenanceId,
                maintenance.ScheduledStartTime,
                maintenance.EstimatedDuration,
                maintenance.MaintenanceType,
                maintenance.AffectedServices));

            return maintenance;
        }

        public void Start()
        {
            StartTime = DateTime.UtcNow;
            Status = MaintenanceStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MaintenanceStartedDomainEvent(
                MaintenanceId,
                StartTime.Value));
        }

        public void Complete()
        {
            CompletedTime = DateTime.UtcNow;
            ActualDuration = CompletedTime.Value - StartTime!.Value;
            Status = MaintenanceStatus.Completed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MaintenanceCompletedDomainEvent(
                MaintenanceId,
                CompletedTime.Value,
                ActualDuration.Value));
        }

        public void UpdateSchedule(DateTime newScheduledStartTime, TimeSpan newEstimatedDuration)
        {
            ScheduledStartTime = newScheduledStartTime;
            EstimatedDuration = newEstimatedDuration;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
```

### SystemBackup (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class SystemBackup : AggregateRoot
    {
        public Guid BackupId { get; private set; }
        public BackupType BackupType { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public long? BackupSize { get; private set; }
        public TimeSpan? Duration { get; private set; }
        public string? BackupLocation { get; private set; }
        public BackupStatus Status { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private SystemBackup() { } // EF Core

        public static SystemBackup Start(BackupType backupType)
        {
            var backup = new SystemBackup
            {
                BackupId = Guid.NewGuid(),
                BackupType = backupType,
                StartedAt = DateTime.UtcNow,
                Status = BackupStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            };

            backup.AddDomainEvent(new BackupStartedDomainEvent(
                backup.BackupId,
                backup.BackupType));

            return backup;
        }

        public void Complete(long backupSize, string backupLocation)
        {
            CompletedAt = DateTime.UtcNow;
            BackupSize = backupSize;
            Duration = CompletedAt.Value - StartedAt;
            BackupLocation = backupLocation;
            Status = BackupStatus.Completed;

            AddDomainEvent(new BackupCompletedDomainEvent(
                BackupId,
                BackupSize.Value,
                Duration.Value,
                BackupLocation));
        }

        public void Fail(string failureReason)
        {
            FailureReason = failureReason;
            Status = BackupStatus.Failed;
            Duration = DateTime.UtcNow - StartedAt;

            AddDomainEvent(new BackupFailedDomainEvent(
                BackupId,
                failureReason));
        }
    }
}
```

### SystemError (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class SystemError : AggregateRoot
    {
        public Guid ErrorId { get; private set; }
        public string ErrorType { get; private set; }
        public string ErrorMessage { get; private set; }
        public string StackTrace { get; private set; }
        public ErrorSeverity Severity { get; private set; }
        public int AffectedAccounts { get; private set; }
        public DateTime OccurredAt { get; private set; }
        public bool IsResolved { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public string? Resolution { get; private set; }

        private SystemError() { } // EF Core

        public static SystemError Create(
            string errorType,
            string errorMessage,
            string stackTrace,
            ErrorSeverity severity,
            int affectedAccounts)
        {
            var error = new SystemError
            {
                ErrorId = Guid.NewGuid(),
                ErrorType = errorType,
                ErrorMessage = errorMessage,
                StackTrace = stackTrace,
                Severity = severity,
                AffectedAccounts = affectedAccounts,
                OccurredAt = DateTime.UtcNow,
                IsResolved = false
            };

            error.AddDomainEvent(new SystemErrorOccurredDomainEvent(
                error.ErrorId,
                error.ErrorType,
                error.ErrorMessage,
                error.StackTrace,
                error.Severity,
                error.AffectedAccounts));

            return error;
        }

        public void Resolve(string resolution)
        {
            IsResolved = true;
            ResolvedAt = DateTime.UtcNow;
            Resolution = resolution;
        }
    }
}
```

### PerformanceMetric (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class PerformanceMetric : AggregateRoot
    {
        public Guid MetricId { get; private set; }
        public string MetricName { get; private set; }
        public double ThresholdValue { get; private set; }
        public double ActualValue { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime RecordedAt { get; private set; }
        public bool ThresholdExceeded { get; private set; }
        public DateTime? AcknowledgedAt { get; private set; }

        private PerformanceMetric() { } // EF Core

        public static PerformanceMetric Record(
            string metricName,
            double thresholdValue,
            double actualValue,
            TimeSpan duration)
        {
            var metric = new PerformanceMetric
            {
                MetricId = Guid.NewGuid(),
                MetricName = metricName,
                ThresholdValue = thresholdValue,
                ActualValue = actualValue,
                Duration = duration,
                RecordedAt = DateTime.UtcNow,
                ThresholdExceeded = actualValue > thresholdValue
            };

            if (metric.ThresholdExceeded)
            {
                metric.AddDomainEvent(new PerformanceThresholdExceededDomainEvent(
                    metric.MetricName,
                    metric.ThresholdValue,
                    metric.ActualValue,
                    metric.Duration));
            }

            return metric;
        }

        public void Acknowledge()
        {
            AcknowledgedAt = DateTime.UtcNow;
        }
    }
}
```

## Enumerations

### MaintenanceType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum MaintenanceType
    {
        Planned = 0,
        Emergency = 1
    }
}
```

### MaintenanceStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum MaintenanceStatus
    {
        Scheduled = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}
```

### BackupType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum BackupType
    {
        Full = 0,
        Incremental = 1,
        Differential = 2
    }
}
```

### BackupStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum BackupStatus
    {
        Scheduled = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3
    }
}
```

### ErrorSeverity

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ErrorSeverity
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
}
```

## Domain Events

### MaintenanceScheduledDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MaintenanceScheduledDomainEvent(
        Guid MaintenanceId,
        DateTime ScheduledStartTime,
        TimeSpan EstimatedDuration,
        MaintenanceType MaintenanceType,
        List<string> AffectedServices) : DomainEvent;
}
```

### MaintenanceStartedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MaintenanceStartedDomainEvent(
        Guid MaintenanceId,
        DateTime StartTime) : DomainEvent;
}
```

### MaintenanceCompletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MaintenanceCompletedDomainEvent(
        Guid MaintenanceId,
        DateTime CompletedTime,
        TimeSpan ActualDuration) : DomainEvent;
}
```

### BackupStartedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record BackupStartedDomainEvent(
        Guid BackupId,
        BackupType BackupType) : DomainEvent;
}
```

### BackupCompletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record BackupCompletedDomainEvent(
        Guid BackupId,
        long BackupSize,
        TimeSpan Duration,
        string BackupLocation) : DomainEvent;
}
```

### BackupFailedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record BackupFailedDomainEvent(
        Guid BackupId,
        string FailureReason) : DomainEvent;
}
```

### SystemErrorOccurredDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record SystemErrorOccurredDomainEvent(
        Guid ErrorId,
        string ErrorType,
        string ErrorMessage,
        string StackTrace,
        ErrorSeverity Severity,
        int AffectedAccounts) : DomainEvent;
}
```

### PerformanceThresholdExceededDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record PerformanceThresholdExceededDomainEvent(
        string MetricName,
        double ThresholdValue,
        double ActualValue,
        TimeSpan Duration) : DomainEvent;
}
```

## MediatR Commands

### ScheduleMaintenanceCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record ScheduleMaintenanceCommand(
        DateTime ScheduledStartTime,
        TimeSpan EstimatedDuration,
        MaintenanceType MaintenanceType,
        List<string> AffectedServices) : IRequest<ScheduleMaintenanceResponse>;

    public record ScheduleMaintenanceResponse(
        Guid MaintenanceId,
        DateTime ScheduledStartTime,
        MaintenanceStatus Status);
}
```

### StartMaintenanceCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record StartMaintenanceCommand(
        Guid MaintenanceId) : IRequest<Unit>;
}
```

### CompleteMaintenanceCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record CompleteMaintenanceCommand(
        Guid MaintenanceId) : IRequest<Unit>;
}
```

### UpdateMaintenanceScheduleCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record UpdateMaintenanceScheduleCommand(
        Guid MaintenanceId,
        DateTime NewScheduledStartTime,
        TimeSpan NewEstimatedDuration) : IRequest<Unit>;
}
```

### StartBackupCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record StartBackupCommand(
        BackupType BackupType) : IRequest<StartBackupResponse>;

    public record StartBackupResponse(
        Guid BackupId,
        BackupStatus Status,
        DateTime StartedAt);
}
```

### CompleteBackupCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record CompleteBackupCommand(
        Guid BackupId,
        long BackupSize,
        string BackupLocation) : IRequest<Unit>;
}
```

### FailBackupCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record FailBackupCommand(
        Guid BackupId,
        string FailureReason) : IRequest<Unit>;
}
```

### RecordSystemErrorCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record RecordSystemErrorCommand(
        string ErrorType,
        string ErrorMessage,
        string StackTrace,
        ErrorSeverity Severity,
        int AffectedAccounts) : IRequest<RecordSystemErrorResponse>;

    public record RecordSystemErrorResponse(
        Guid ErrorId,
        ErrorSeverity Severity,
        DateTime OccurredAt);
}
```

### ResolveSystemErrorCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record ResolveSystemErrorCommand(
        Guid ErrorId,
        string Resolution) : IRequest<Unit>;
}
```

### RecordPerformanceMetricCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record RecordPerformanceMetricCommand(
        string MetricName,
        double ThresholdValue,
        double ActualValue,
        TimeSpan Duration) : IRequest<RecordPerformanceMetricResponse>;

    public record RecordPerformanceMetricResponse(
        Guid MetricId,
        bool ThresholdExceeded);
}
```

### AcknowledgePerformanceMetricCommand

```csharp
namespace StartupStarter.Application.System.Commands
{
    public record AcknowledgePerformanceMetricCommand(
        Guid MetricId) : IRequest<Unit>;
}
```

## MediatR Queries

### GetMaintenanceWindowQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetMaintenanceWindowQuery(
        Guid MaintenanceId) : IRequest<MaintenanceWindowDto?>;
}
```

### GetScheduledMaintenanceQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetScheduledMaintenanceQuery(
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        MaintenanceType? MaintenanceType = null) : IRequest<List<MaintenanceWindowDto>>;
}
```

### GetBackupHistoryQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetBackupHistoryQuery(
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        BackupType? BackupType = null,
        BackupStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<SystemBackupDto>>;
}
```

### GetBackupByIdQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetBackupByIdQuery(
        Guid BackupId) : IRequest<SystemBackupDto?>;
}
```

### GetSystemErrorsQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetSystemErrorsQuery(
        ErrorSeverity? Severity = null,
        bool? IsResolved = null,
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<SystemErrorDto>>;
}
```

### GetSystemErrorByIdQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetSystemErrorByIdQuery(
        Guid ErrorId) : IRequest<SystemErrorDto?>;
}
```

### GetPerformanceMetricsQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetPerformanceMetricsQuery(
        string? MetricName = null,
        bool? ThresholdExceeded = null,
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<PerformanceMetricDto>>;
}
```

### GetSystemHealthQuery

```csharp
namespace StartupStarter.Application.System.Queries
{
    public record GetSystemHealthQuery() : IRequest<SystemHealthDto>;
}
```

## DTOs

### MaintenanceWindowDto

```csharp
namespace StartupStarter.Application.System.DTOs
{
    public record MaintenanceWindowDto(
        Guid MaintenanceId,
        DateTime ScheduledStartTime,
        TimeSpan EstimatedDuration,
        MaintenanceType MaintenanceType,
        List<string> AffectedServices,
        DateTime? StartTime,
        DateTime? CompletedTime,
        TimeSpan? ActualDuration,
        MaintenanceStatus Status,
        DateTime CreatedAt);
}
```

### SystemBackupDto

```csharp
namespace StartupStarter.Application.System.DTOs
{
    public record SystemBackupDto(
        Guid BackupId,
        BackupType BackupType,
        DateTime StartedAt,
        DateTime? CompletedAt,
        long? BackupSize,
        TimeSpan? Duration,
        string? BackupLocation,
        BackupStatus Status,
        string? FailureReason);
}
```

### SystemErrorDto

```csharp
namespace StartupStarter.Application.System.DTOs
{
    public record SystemErrorDto(
        Guid ErrorId,
        string ErrorType,
        string ErrorMessage,
        string StackTrace,
        ErrorSeverity Severity,
        int AffectedAccounts,
        DateTime OccurredAt,
        bool IsResolved,
        DateTime? ResolvedAt,
        string? Resolution);
}
```

### PerformanceMetricDto

```csharp
namespace StartupStarter.Application.System.DTOs
{
    public record PerformanceMetricDto(
        Guid MetricId,
        string MetricName,
        double ThresholdValue,
        double ActualValue,
        TimeSpan Duration,
        DateTime RecordedAt,
        bool ThresholdExceeded,
        DateTime? AcknowledgedAt);
}
```

### SystemHealthDto

```csharp
namespace StartupStarter.Application.System.DTOs
{
    public record SystemHealthDto(
        string Status,
        DateTime CheckedAt,
        int ActiveErrors,
        int CriticalErrors,
        int ActiveMaintenanceWindows,
        DateTime? LastBackup,
        BackupStatus? LastBackupStatus,
        Dictionary<string, double> PerformanceMetrics);
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

### IMaintenanceWindowRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IMaintenanceWindowRepository
    {
        Task<MaintenanceWindow?> GetByIdAsync(Guid maintenanceId, CancellationToken cancellationToken = default);
        Task<List<MaintenanceWindow>> GetScheduledAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            MaintenanceType? maintenanceType = null,
            CancellationToken cancellationToken = default);
        Task<List<MaintenanceWindow>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task AddAsync(MaintenanceWindow maintenanceWindow, CancellationToken cancellationToken = default);
        Task UpdateAsync(MaintenanceWindow maintenanceWindow, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### ISystemBackupRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface ISystemBackupRepository
    {
        Task<SystemBackup?> GetByIdAsync(Guid backupId, CancellationToken cancellationToken = default);
        Task<PaginatedList<SystemBackup>> GetPagedAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            BackupType? backupType = null,
            BackupStatus? status = null,
            int pageNumber = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
        Task<SystemBackup?> GetLastBackupAsync(CancellationToken cancellationToken = default);
        Task AddAsync(SystemBackup backup, CancellationToken cancellationToken = default);
        Task UpdateAsync(SystemBackup backup, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### ISystemErrorRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface ISystemErrorRepository
    {
        Task<SystemError?> GetByIdAsync(Guid errorId, CancellationToken cancellationToken = default);
        Task<PaginatedList<SystemError>> GetPagedAsync(
            ErrorSeverity? severity = null,
            bool? isResolved = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
        Task<int> GetActiveErrorsCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetCriticalErrorsCountAsync(CancellationToken cancellationToken = default);
        Task AddAsync(SystemError error, CancellationToken cancellationToken = default);
        Task UpdateAsync(SystemError error, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IPerformanceMetricRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IPerformanceMetricRepository
    {
        Task<PerformanceMetric?> GetByIdAsync(Guid metricId, CancellationToken cancellationToken = default);
        Task<PaginatedList<PerformanceMetric>> GetPagedAsync(
            string? metricName = null,
            bool? thresholdExceeded = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
        Task<Dictionary<string, double>> GetLatestMetricsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(PerformanceMetric metric, CancellationToken cancellationToken = default);
        Task UpdateAsync(PerformanceMetric metric, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```
