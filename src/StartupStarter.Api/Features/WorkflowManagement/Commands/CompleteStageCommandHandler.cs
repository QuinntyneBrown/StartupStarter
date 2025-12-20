using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WorkflowManagement.Commands;

public class CompleteStageCommandHandler : IRequestHandler<CompleteStageCommand, WorkflowStageDto>
{
    private readonly IStartupStarterContext _context;

    public CompleteStageCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WorkflowStageDto> Handle(CompleteStageCommand request, CancellationToken cancellationToken)
    {
        var workflowStage = await _context.WorkflowStages
            .FirstOrDefaultAsync(ws => ws.WorkflowStageId == request.StageId, cancellationToken);

        if (workflowStage == null)
            throw new InvalidOperationException($"Workflow stage with ID {request.StageId} not found");

        workflowStage.Complete(request.CompletedBy);
        await _context.SaveChangesAsync(cancellationToken);

        return workflowStage.ToDto();
    }
}
