using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;

namespace StartupStarter.Api.Features.SystemManagement.Commands;

public class StartMaintenanceCommand : IRequest<SystemMaintenanceDto>
{
    public string MaintenanceId { get; set; } = string.Empty;
}
