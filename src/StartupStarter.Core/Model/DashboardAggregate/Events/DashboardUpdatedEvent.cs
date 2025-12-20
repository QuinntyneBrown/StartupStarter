namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardUpdatedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedFields { get; set; } = new();
    public Dictionary<string, object> PreviousValues { get; set; } = new();
    public DateTime Timestamp { get; set; }
}
