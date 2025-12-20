namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountSuspendedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string SuspendedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public TimeSpan? SuspensionDuration { get; set; }
    public int AffectedUserCount { get; set; }
    public int AffectedProfileCount { get; set; }
    public DateTime Timestamp { get; set; }
}
