using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.SystemManagement.Commands;

public class StartMaintenanceCommandHandler : IRequestHandler<StartMaintenanceCommand, SystemMaintenanceDto>
{
    private readonly IStartupStarterContext _context;

    public StartMaintenanceCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<SystemMaintenanceDto> Handle(StartMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var maintenance = await _context.SystemMaintenances
            .FirstOrDefaultAsync(m => m.MaintenanceId == request.MaintenanceId, cancellationToken);

        if (maintenance == null)
            throw new InvalidOperationException($"Maintenance {request.MaintenanceId} not found");

        maintenance.Start();
        await _context.SaveChangesAsync(cancellationToken);

        return maintenance.ToDto();
    }
}
