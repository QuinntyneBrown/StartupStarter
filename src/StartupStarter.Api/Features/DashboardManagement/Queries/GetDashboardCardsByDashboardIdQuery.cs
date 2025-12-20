using MediatR;
using StartupStarter.Api.Features.DashboardManagement.Dtos;

namespace StartupStarter.Api.Features.DashboardManagement.Queries;

public class GetDashboardCardsByDashboardIdQuery : IRequest<List<DashboardCardDto>>
{
    public string DashboardId { get; set; } = string.Empty;
}
