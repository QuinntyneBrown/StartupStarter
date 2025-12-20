using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WorkflowManagement.Queries;

public class GetWorkflowByIdQueryHandler : IRequestHandler<GetWorkflowByIdQuery, WorkflowDto?>
{
    private readonly IStartupStarterContext _context;

    public GetWorkflowByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WorkflowDto?> Handle(GetWorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        var workflow = await _context.Workflows
            .FirstOrDefaultAsync(w => w.WorkflowId == request.WorkflowId, cancellationToken);

        return workflow?.ToDto();
    }
}
