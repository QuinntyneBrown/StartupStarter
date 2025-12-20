using StartupStarter.Core.Model.MaintenanceAggregate.Enums;
using StartupStarter.Core.Model.MaintenanceAggregate.Events;

namespace StartupStarter.Core.Model.MaintenanceAggregate.Entities;

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

    // EF Core constructor
    private SystemMaintenance()
    {
        MaintenanceId = string.Empty;
    }

    public SystemMaintenance(string maintenanceId, DateTime scheduledStartTime,
        TimeSpan estimatedDuration, MaintenanceType maintenanceType, List<string> affectedServices)
    {
        if (string.IsNullOrWhiteSpace(maintenanceId))
            throw new ArgumentException("Maintenance ID cannot be empty", nameof(maintenanceId));
        if (scheduledStartTime < DateTime.UtcNow)
            throw new ArgumentException("Scheduled start time cannot be in the past", nameof(scheduledStartTime));
        if (estimatedDuration <= TimeSpan.Zero)
            throw new ArgumentException("Estimated duration must be positive", nameof(estimatedDuration));

        MaintenanceId = maintenanceId;
        ScheduledStartTime = scheduledStartTime;
        EstimatedDuration = estimatedDuration;
        MaintenanceType = maintenanceType;

        if (affectedServices != null && affectedServices.Any())
        {
            _affectedServices.AddRange(affectedServices);
        }

        AddDomainEvent(new SystemMaintenanceScheduledEvent
        {
            MaintenanceId = MaintenanceId,
            ScheduledStartTime = ScheduledStartTime,
            EstimatedDuration = EstimatedDuration,
            MaintenanceType = MaintenanceType,
            AffectedServices = affectedServices ?? new List<string>(),
            Timestamp = DateTime.UtcNow
        });
    }

    public void Start()
    {
        ActualStartTime = DateTime.UtcNow;

        AddDomainEvent(new SystemMaintenanceStartedEvent
        {
            MaintenanceId = MaintenanceId,
            ScheduledStartTime = ScheduledStartTime,
            ActualStartTime = ActualStartTime.Value,
            MaintenanceType = MaintenanceType,
            AffectedServices = _affectedServices.ToList(),
            Timestamp = ActualStartTime.Value
        });
    }

    public void Complete()
    {
        if (!ActualStartTime.HasValue)
            throw new InvalidOperationException("Cannot complete maintenance that has not been started");

        CompletedTime = DateTime.UtcNow;
        ActualDuration = CompletedTime.Value - ActualStartTime.Value;

        AddDomainEvent(new SystemMaintenanceCompletedEvent
        {
            MaintenanceId = MaintenanceId,
            ScheduledStartTime = ScheduledStartTime,
            ActualStartTime = ActualStartTime.Value,
            CompletedTime = CompletedTime.Value,
            EstimatedDuration = EstimatedDuration,
            ActualDuration = ActualDuration.Value,
            MaintenanceType = MaintenanceType,
            Timestamp = CompletedTime.Value
        });
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
