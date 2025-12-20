namespace StartupStarter.Core.Model.WorkflowAggregate.Events;

public class WorkflowCompletedEvent : DomainEvent
{
    public string WorkflowId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CompletedBy { get; set; } = string.Empty;
    public string FinalStatus { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
}
