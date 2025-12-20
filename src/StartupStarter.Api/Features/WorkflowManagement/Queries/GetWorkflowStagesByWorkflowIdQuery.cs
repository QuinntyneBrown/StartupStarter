using MediatR;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;

namespace StartupStarter.Api.Features.WorkflowManagement.Queries;

public class GetWorkflowStagesByWorkflowIdQuery : IRequest<List<WorkflowStageDto>>
{
    public string WorkflowId { get; set; } = string.Empty;
}
