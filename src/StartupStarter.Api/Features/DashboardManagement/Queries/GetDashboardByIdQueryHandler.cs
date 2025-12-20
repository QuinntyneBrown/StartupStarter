using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.DashboardManagement.Queries;

public class GetDashboardByIdQueryHandler : IRequestHandler<GetDashboardByIdQuery, DashboardDto?>
{
    private readonly IStartupStarterContext _context;

    public GetDashboardByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto?> Handle(GetDashboardByIdQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await _context.Dashboards
            .FirstOrDefaultAsync(d => d.DashboardId == request.DashboardId, cancellationToken);

        return dashboard?.ToDto();
    }
}
