namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardCardAddedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AddedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
