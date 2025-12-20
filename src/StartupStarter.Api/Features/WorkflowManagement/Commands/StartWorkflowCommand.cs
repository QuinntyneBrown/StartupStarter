using MediatR;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;

namespace StartupStarter.Api.Features.WorkflowManagement.Commands;

public class StartWorkflowCommand : IRequest<WorkflowDto>
{
    public string ContentId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string WorkflowType { get; set; } = string.Empty;
    public string InitiatedBy { get; set; } = string.Empty;
}
