using MediatR;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;

namespace StartupStarter.Api.Features.WorkflowManagement.Commands;

public class ApproveWorkflowCommand : IRequest<WorkflowApprovalDto>
{
    public string StageId { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string Comments { get; set; } = string.Empty;
    public string ApprovalLevel { get; set; } = string.Empty;
}
