using StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

namespace StartupStarter.Core.Model.DashboardAggregate.Events;

public class DashboardCardRepositionedEvent : DomainEvent
{
    public string DashboardId { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public CardPosition NewPosition { get; set; } = null!;
    public string RepositionedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
