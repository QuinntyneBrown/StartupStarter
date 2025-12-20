namespace StartupStarter.Core.Model.WorkflowAggregate.Entities;

public class WorkflowStage
{
    public string WorkflowStageId { get; private set; }
    public string WorkflowId { get; private set; }
    public string StageName { get; private set; }
    public int StageOrder { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string CompletedBy { get; private set; }
    public bool IsCompleted { get; private set; }

    public Workflow Workflow { get; private set; } = null!;

    // EF Core constructor
    private WorkflowStage()
    {
        WorkflowStageId = string.Empty;
        WorkflowId = string.Empty;
        StageName = string.Empty;
        CompletedBy = string.Empty;
    }

    public WorkflowStage(string workflowStageId, string workflowId, string stageName, int stageOrder)
    {
        if (string.IsNullOrWhiteSpace(workflowStageId))
            throw new ArgumentException("WorkflowStage ID cannot be empty", nameof(workflowStageId));
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be empty", nameof(workflowId));
        if (string.IsNullOrWhiteSpace(stageName))
            throw new ArgumentException("Stage name cannot be empty", nameof(stageName));
        if (stageOrder < 1)
            throw new ArgumentException("Stage order must be greater than 0", nameof(stageOrder));

        WorkflowStageId = workflowStageId;
        WorkflowId = workflowId;
        StageName = stageName;
        StageOrder = stageOrder;
        CompletedBy = string.Empty;
        IsCompleted = false;
    }

    public void Complete(string completedBy)
    {
        if (string.IsNullOrWhiteSpace(completedBy))
            throw new ArgumentException("CompletedBy cannot be empty", nameof(completedBy));

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = completedBy;
    }
}
