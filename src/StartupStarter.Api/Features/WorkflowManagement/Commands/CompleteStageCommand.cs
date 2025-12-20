using MediatR;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;

namespace StartupStarter.Api.Features.WorkflowManagement.Commands;

public class CompleteStageCommand : IRequest<WorkflowStageDto>
{
    public string StageId { get; set; } = string.Empty;
    public string CompletedBy { get; set; } = string.Empty;
}
