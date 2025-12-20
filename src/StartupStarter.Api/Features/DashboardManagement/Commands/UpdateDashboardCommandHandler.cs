using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.DashboardManagement.Commands;

public class UpdateDashboardCommandHandler : IRequestHandler<UpdateDashboardCommand, DashboardDto>
{
    private readonly IStartupStarterContext _context;

    public UpdateDashboardCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(UpdateDashboardCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _context.Dashboards
            .FirstOrDefaultAsync(d => d.DashboardId == request.DashboardId, cancellationToken);

        if (dashboard == null)
            throw new InvalidOperationException($"Dashboard with ID {request.DashboardId} not found");

        dashboard.Update(request.UpdatedFields, request.UpdatedBy);
        await _context.SaveChangesAsync(cancellationToken);

        return dashboard.ToDto();
    }
}
