using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Features.WorkflowManagement.Dtos;

public static class WorkflowExtensions
{
    public static WorkflowDto ToDto(this Workflow workflow)
    {
        return new WorkflowDto
        {
            WorkflowId = workflow.WorkflowId,
            ContentId = workflow.ContentId,
            WorkflowType = workflow.WorkflowType,
            Status = workflow.IsCompleted ? "Completed" : "InProgress",
            StartedAt = workflow.StartedAt,
            CompletedAt = workflow.CompletedAt,
            CancelledAt = workflow.CancelledAt
        };
    }

    public static WorkflowStageDto ToDto(this WorkflowStage workflowStage)
    {
        return new WorkflowStageDto
        {
            StageId = workflowStage.WorkflowStageId,
            WorkflowId = workflowStage.WorkflowId,
            StageName = workflowStage.StageName,
            Status = workflowStage.IsCompleted ? "Completed" : "Pending",
            AssignedTo = workflowStage.CompletedBy,
            StartedAt = null,
            CompletedAt = workflowStage.CompletedAt
        };
    }

    public static WorkflowApprovalDto ToDto(this WorkflowApproval workflowApproval)
    {
        return new WorkflowApprovalDto
        {
            ApprovalId = workflowApproval.WorkflowApprovalId,
            StageId = workflowApproval.WorkflowId,
            ApprovedBy = workflowApproval.ApprovedBy,
            Decision = workflowApproval.IsApproved ? "Approved" : "Rejected",
            Comments = workflowApproval.Comments,
            DecidedAt = workflowApproval.ApprovalDate
        };
    }
}
