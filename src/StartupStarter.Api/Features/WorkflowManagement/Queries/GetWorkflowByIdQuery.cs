using MediatR;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;

namespace StartupStarter.Api.Features.WorkflowManagement.Queries;

public class GetWorkflowByIdQuery : IRequest<WorkflowDto?>
{
    public string WorkflowId { get; set; } = string.Empty;
}
