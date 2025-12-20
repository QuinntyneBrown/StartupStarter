using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Core.Model.MaintenanceAggregate.Enums;

namespace StartupStarter.Api.Features.SystemManagement.Commands;

public class ScheduleMaintenanceCommand : IRequest<SystemMaintenanceDto>
{
    public DateTime ScheduledStartTime { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public List<string> AffectedServices { get; set; } = new();
}
