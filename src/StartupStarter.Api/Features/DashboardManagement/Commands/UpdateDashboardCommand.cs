using MediatR;
using StartupStarter.Api.Features.DashboardManagement.Dtos;

namespace StartupStarter.Api.Features.DashboardManagement.Commands;

public class UpdateDashboardCommand : IRequest<DashboardDto>
{
    public string DashboardId { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedFields { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
}
