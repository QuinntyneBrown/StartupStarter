using MediatR;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.WorkflowAggregate.Entities;

namespace StartupStarter.Api.Features.WorkflowManagement.Commands;

public class StartWorkflowCommandHandler : IRequestHandler<StartWorkflowCommand, WorkflowDto>
{
    private readonly IStartupStarterContext _context;

    public StartWorkflowCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WorkflowDto> Handle(StartWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflowId = Guid.NewGuid().ToString();

        var workflow = new Workflow(
            workflowId,
            request.ContentId,
            request.AccountId,
            request.WorkflowType,
            request.InitiatedBy
        );

        _context.Workflows.Add(workflow);
        await _context.SaveChangesAsync(cancellationToken);

        return workflow.ToDto();
    }
}
