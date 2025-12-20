namespace StartupStarter.Core.Model.WorkflowAggregate.Events;

public class WorkflowStageCompletedEvent : DomainEvent
{
    public string WorkflowId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public string CompletedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
