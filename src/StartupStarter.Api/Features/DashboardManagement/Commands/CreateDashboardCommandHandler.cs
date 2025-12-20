using MediatR;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.DashboardAggregate.Entities;

namespace StartupStarter.Api.Features.DashboardManagement.Commands;

public class CreateDashboardCommandHandler : IRequestHandler<CreateDashboardCommand, DashboardDto>
{
    private readonly IStartupStarterContext _context;

    public CreateDashboardCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(CreateDashboardCommand request, CancellationToken cancellationToken)
    {
        var dashboardId = Guid.NewGuid().ToString();

        var dashboard = new Dashboard(
            dashboardId,
            request.DashboardName,
            request.ProfileId,
            request.AccountId,
            request.CreatedBy,
            request.IsDefault,
            request.Template,
            request.LayoutType
        );

        _context.Dashboards.Add(dashboard);
        await _context.SaveChangesAsync(cancellationToken);

        return dashboard.ToDto();
    }
}
