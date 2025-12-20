using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.SystemManagement.Queries;

public class GetMaintenanceByIdQueryHandler : IRequestHandler<GetMaintenanceByIdQuery, SystemMaintenanceDto?>
{
    private readonly IStartupStarterContext _context;

    public GetMaintenanceByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<SystemMaintenanceDto?> Handle(GetMaintenanceByIdQuery request, CancellationToken cancellationToken)
    {
        var maintenance = await _context.SystemMaintenances
            .FirstOrDefaultAsync(m => m.MaintenanceId == request.MaintenanceId, cancellationToken);

        return maintenance?.ToDto();
    }
}
