using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Features.WorkflowManagement.Commands;

public class ApproveWorkflowCommandHandler : IRequestHandler<ApproveWorkflowCommand, WorkflowApprovalDto>
{
    private readonly IStartupStarterContext _context;

    public ApproveWorkflowCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WorkflowApprovalDto> Handle(ApproveWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowStage = await _context.WorkflowStages
            .FirstOrDefaultAsync(ws => ws.WorkflowStageId == request.StageId, cancellationToken);

        if (workflowStage == null)
            throw new InvalidOperationException($"Workflow stage with ID {request.StageId} not found");

        var approvalId = Guid.NewGuid().ToString();
        var rejectionReason = request.IsApproved ? string.Empty : "Rejected";

        var workflowApproval = new WorkflowApproval(
            approvalId,
            workflowStage.WorkflowId,
            request.ApprovedBy,
            request.ApprovalLevel,
            request.IsApproved,
            request.Comments,
            rejectionReason
        );

        _context.WorkflowApprovals.Add(workflowApproval);
        await _context.SaveChangesAsync(cancellationToken);

        return workflowApproval.ToDto();
    }
}
