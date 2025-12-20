using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WorkflowManagement.Queries;

public class GetWorkflowStagesByWorkflowIdQueryHandler : IRequestHandler<GetWorkflowStagesByWorkflowIdQuery, List<WorkflowStageDto>>
{
    private readonly IStartupStarterContext _context;

    public GetWorkflowStagesByWorkflowIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<WorkflowStageDto>> Handle(GetWorkflowStagesByWorkflowIdQuery request, CancellationToken cancellationToken)
    {
        var workflowStages = await _context.WorkflowStages
            .Where(ws => ws.WorkflowId == request.WorkflowId)
            .OrderBy(ws => ws.StageOrder)
            .ToListAsync(cancellationToken);

        return workflowStages.Select(ws => ws.ToDto()).ToList();
    }
}
