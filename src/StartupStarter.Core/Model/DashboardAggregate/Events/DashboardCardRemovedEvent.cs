namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardCardRemovedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RemovedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
