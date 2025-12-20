using MediatR;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Core.Model.DashboardAggregate.Enums;

namespace StartupStarter.Api.Features.DashboardManagement.Commands;

public class CreateDashboardCommand : IRequest<DashboardDto>
{
    public string DashboardName { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string Template { get; set; } = string.Empty;
    public LayoutType LayoutType { get; set; }
}
