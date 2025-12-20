namespace StartupStarter.Core.Model.RoleAggregate.Events;

public class RolePermissionsUpdatedEvent : DomainEvent
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> AddedPermissions { get; set; } = new();
    public List<string> RemovedPermissions { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
