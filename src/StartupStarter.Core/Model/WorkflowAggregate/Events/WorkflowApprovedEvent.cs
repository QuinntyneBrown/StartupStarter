namespace StartupStarter.Core.Model.WorkflowAggregate.Events;

public class WorkflowApprovedEvent : DomainEvent
{
    public string WorkflowId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public string ApprovalLevel { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
