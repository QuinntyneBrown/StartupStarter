namespace StartupStarter.Core.Model.WorkflowAggregate.Events;

public class WorkflowStartedEvent : DomainEvent
{
    public string WorkflowId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string WorkflowType { get; set; } = string.Empty;
    public string InitiatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
