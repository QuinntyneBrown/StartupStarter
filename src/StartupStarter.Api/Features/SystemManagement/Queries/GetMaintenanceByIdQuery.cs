using MediatR;
using StartupStarter.Api.Features.SystemManagement.Dtos;

namespace StartupStarter.Api.Features.SystemManagement.Queries;

public class GetMaintenanceByIdQuery : IRequest<SystemMaintenanceDto?>
{
    public string MaintenanceId { get; set; } = string.Empty;
}
