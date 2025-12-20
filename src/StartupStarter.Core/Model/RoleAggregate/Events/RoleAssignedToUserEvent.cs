namespace StartupStarter.Core.Model.RoleAggregate.Events;

public class RoleAssignedToUserEvent : DomainEvent
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
