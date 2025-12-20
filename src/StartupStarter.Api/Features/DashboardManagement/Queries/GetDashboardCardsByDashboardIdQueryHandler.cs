using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.DashboardManagement.Queries;

public class GetDashboardCardsByDashboardIdQueryHandler : IRequestHandler<GetDashboardCardsByDashboardIdQuery, List<DashboardCardDto>>
{
    private readonly IStartupStarterContext _context;

    public GetDashboardCardsByDashboardIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<DashboardCardDto>> Handle(GetDashboardCardsByDashboardIdQuery request, CancellationToken cancellationToken)
    {
        var cards = await _context.DashboardCards
            .Where(c => c.DashboardId == request.DashboardId)
            .ToListAsync(cancellationToken);

        return cards.Select(c => c.ToDto()).ToList();
    }
}
