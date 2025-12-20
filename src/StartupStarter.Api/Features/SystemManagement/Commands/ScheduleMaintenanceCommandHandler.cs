using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;

namespace StartupStarter.Api.Features.SystemManagement.Commands;

public class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, SystemMaintenanceDto>
{
    private readonly IStartupStarterContext _context;

    public ScheduleMaintenanceCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<SystemMaintenanceDto> Handle(ScheduleMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var maintenanceId = Guid.NewGuid().ToString();

        var maintenance = new SystemMaintenance(
            maintenanceId,
            request.ScheduledStartTime,
            request.EstimatedDuration,
            request.MaintenanceType,
            request.AffectedServices
        );

        _context.SystemMaintenances.Add(maintenance);
        await _context.SaveChangesAsync(cancellationToken);

        return maintenance.ToDto();
    }
}
