using MediatR;
using StartupStarter.Api.Features.DashboardManagement.Dtos;

namespace StartupStarter.Api.Features.DashboardManagement.Queries;

public class GetDashboardByIdQuery : IRequest<DashboardDto?>
{
    public string DashboardId { get; set; } = string.Empty;
}
