namespace StartupStarter.Core.Model.WorkflowAggregate.Events;

public class WorkflowReassignedEvent : DomainEvent
{
    public string WorkflowId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string PreviousAssigneeId { get; set; } = string.Empty;
    public string NewAssigneeId { get; set; } = string.Empty;
    public string ReassignedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
