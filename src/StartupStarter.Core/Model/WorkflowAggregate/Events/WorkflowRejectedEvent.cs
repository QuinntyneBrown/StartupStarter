namespace StartupStarter.Core.Model.WorkflowAggregate.Events;

public class WorkflowRejectedEvent : DomainEvent
{
    public string WorkflowId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RejectedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
