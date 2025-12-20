using StartupStarter.Core.Model.WorkflowAggregate.Events;

namespace StartupStarter.Core.Model.WorkflowAggregate.Entities;

public class Workflow
{
    public string WorkflowId { get; private set; }
    public string ContentId { get; private set; }
    public string AccountId { get; private set; }
    public string WorkflowType { get; private set; }
    public string InitiatedBy { get; private set; }
    public string CurrentAssigneeId { get; private set; }
    public string CurrentStage { get; private set; }
    public string FinalStatus { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public bool IsCompleted { get; private set; }

    private readonly List<WorkflowStage> _stages = new();
    public IReadOnlyCollection<WorkflowStage> Stages => _stages.AsReadOnly();

    private readonly List<WorkflowApproval> _approvals = new();
    public IReadOnlyCollection<WorkflowApproval> Approvals => _approvals.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private Workflow()
    {
        WorkflowId = string.Empty;
        ContentId = string.Empty;
        AccountId = string.Empty;
        WorkflowType = string.Empty;
        InitiatedBy = string.Empty;
        CurrentAssigneeId = string.Empty;
        CurrentStage = string.Empty;
        FinalStatus = string.Empty;
    }

    public Workflow(string workflowId, string contentId, string accountId, string workflowType, string initiatedBy)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be empty", nameof(workflowId));
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(workflowType))
            throw new ArgumentException("Workflow type cannot be empty", nameof(workflowType));
        if (string.IsNullOrWhiteSpace(initiatedBy))
            throw new ArgumentException("InitiatedBy cannot be empty", nameof(initiatedBy));

        WorkflowId = workflowId;
        ContentId = contentId;
        AccountId = accountId;
        WorkflowType = workflowType;
        InitiatedBy = initiatedBy;
        CurrentAssigneeId = initiatedBy;
        CurrentStage = string.Empty;
        FinalStatus = string.Empty;
        StartedAt = DateTime.UtcNow;
        IsCompleted = false;

        AddDomainEvent(new WorkflowStartedEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            WorkflowType = WorkflowType,
            InitiatedBy = InitiatedBy,
            Timestamp = StartedAt
        });
    }

    public void CompleteStage(string stageName, string completedBy)
    {
        if (string.IsNullOrWhiteSpace(stageName))
            throw new ArgumentException("Stage name cannot be empty", nameof(stageName));
        if (string.IsNullOrWhiteSpace(completedBy))
            throw new ArgumentException("CompletedBy cannot be empty", nameof(completedBy));

        CurrentStage = stageName;

        AddDomainEvent(new WorkflowStageCompletedEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            StageName = stageName,
            CompletedBy = completedBy,
            Timestamp = DateTime.UtcNow
        });
    }

    public void Approve(string approvedBy, string approvalLevel, string comments)
    {
        if (string.IsNullOrWhiteSpace(approvedBy))
            throw new ArgumentException("ApprovedBy cannot be empty", nameof(approvedBy));
        if (string.IsNullOrWhiteSpace(approvalLevel))
            throw new ArgumentException("Approval level cannot be empty", nameof(approvalLevel));

        AddDomainEvent(new WorkflowApprovedEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            ApprovedBy = approvedBy,
            ApprovalLevel = approvalLevel,
            Comments = comments ?? string.Empty,
            Timestamp = DateTime.UtcNow
        });
    }

    public void Reject(string rejectedBy, string rejectionReason, string comments)
    {
        if (string.IsNullOrWhiteSpace(rejectedBy))
            throw new ArgumentException("RejectedBy cannot be empty", nameof(rejectedBy));
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason cannot be empty", nameof(rejectionReason));

        AddDomainEvent(new WorkflowRejectedEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            RejectedBy = rejectedBy,
            RejectionReason = rejectionReason,
            Comments = comments ?? string.Empty,
            Timestamp = DateTime.UtcNow
        });
    }

    public void Reassign(string previousAssigneeId, string newAssigneeId, string reassignedBy)
    {
        if (string.IsNullOrWhiteSpace(previousAssigneeId))
            throw new ArgumentException("Previous assignee ID cannot be empty", nameof(previousAssigneeId));
        if (string.IsNullOrWhiteSpace(newAssigneeId))
            throw new ArgumentException("New assignee ID cannot be empty", nameof(newAssigneeId));
        if (string.IsNullOrWhiteSpace(reassignedBy))
            throw new ArgumentException("ReassignedBy cannot be empty", nameof(reassignedBy));

        CurrentAssigneeId = newAssigneeId;

        AddDomainEvent(new WorkflowReassignedEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            PreviousAssigneeId = previousAssigneeId,
            NewAssigneeId = newAssigneeId,
            ReassignedBy = reassignedBy,
            Timestamp = DateTime.UtcNow
        });
    }

    public void Complete(string completedBy, string finalStatus)
    {
        if (string.IsNullOrWhiteSpace(completedBy))
            throw new ArgumentException("CompletedBy cannot be empty", nameof(completedBy));
        if (string.IsNullOrWhiteSpace(finalStatus))
            throw new ArgumentException("Final status cannot be empty", nameof(finalStatus));

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        FinalStatus = finalStatus;
        Duration = CompletedAt.Value - StartedAt;

        AddDomainEvent(new WorkflowCompletedEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            CompletedBy = completedBy,
            FinalStatus = finalStatus,
            Duration = Duration.Value,
            Timestamp = CompletedAt.Value
        });
    }

    public void Cancel(string cancelledBy, string reason)
    {
        if (string.IsNullOrWhiteSpace(cancelledBy))
            throw new ArgumentException("CancelledBy cannot be empty", nameof(cancelledBy));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be empty", nameof(reason));

        IsCompleted = true;
        CancelledAt = DateTime.UtcNow;
        Duration = CancelledAt.Value - StartedAt;

        AddDomainEvent(new WorkflowCancelledEvent
        {
            WorkflowId = WorkflowId,
            ContentId = ContentId,
            AccountId = AccountId,
            CancelledBy = cancelledBy,
            Reason = reason,
            Timestamp = CancelledAt.Value
        });
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
