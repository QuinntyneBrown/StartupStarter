namespace StartupStarter.Core.Model.RoleAggregate.Events;

public class RoleDeletedEvent : DomainEvent
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public int AffectedUserCount { get; set; }
    public DateTime Timestamp { get; set; }
}
