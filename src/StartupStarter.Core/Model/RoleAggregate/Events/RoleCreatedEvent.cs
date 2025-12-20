namespace StartupStarter.Core.Model.RoleAggregate.Events;

public class RoleCreatedEvent : DomainEvent
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
