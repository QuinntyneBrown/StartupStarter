namespace StartupStarter.Core.Model.WorkflowAggregate.Entities;

public class WorkflowApproval
{
    public string WorkflowApprovalId { get; private set; }
    public string WorkflowId { get; private set; }
    public string ApprovedBy { get; private set; }
    public string ApprovalLevel { get; private set; }
    public string Comments { get; private set; }
    public bool IsApproved { get; private set; }
    public string RejectionReason { get; private set; }
    public DateTime ApprovalDate { get; private set; }

    public Workflow Workflow { get; private set; } = null!;

    // EF Core constructor
    private WorkflowApproval()
    {
        WorkflowApprovalId = string.Empty;
        WorkflowId = string.Empty;
        ApprovedBy = string.Empty;
        ApprovalLevel = string.Empty;
        Comments = string.Empty;
        RejectionReason = string.Empty;
    }

    public WorkflowApproval(string workflowApprovalId, string workflowId, string approvedBy,
        string approvalLevel, bool isApproved, string comments, string rejectionReason = "")
    {
        if (string.IsNullOrWhiteSpace(workflowApprovalId))
            throw new ArgumentException("WorkflowApproval ID cannot be empty", nameof(workflowApprovalId));
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be empty", nameof(workflowId));
        if (string.IsNullOrWhiteSpace(approvedBy))
            throw new ArgumentException("ApprovedBy cannot be empty", nameof(approvedBy));
        if (string.IsNullOrWhiteSpace(approvalLevel))
            throw new ArgumentException("Approval level cannot be empty", nameof(approvalLevel));

        WorkflowApprovalId = workflowApprovalId;
        WorkflowId = workflowId;
        ApprovedBy = approvedBy;
        ApprovalLevel = approvalLevel;
        IsApproved = isApproved;
        Comments = comments ?? string.Empty;
        RejectionReason = rejectionReason ?? string.Empty;
        ApprovalDate = DateTime.UtcNow;
    }
}
