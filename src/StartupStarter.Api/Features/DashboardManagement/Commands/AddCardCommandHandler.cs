using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

namespace StartupStarter.Api.Features.DashboardManagement.Commands;

public class AddCardCommandHandler : IRequestHandler<AddCardCommand, DashboardCardDto>
{
    private readonly IStartupStarterContext _context;

    public AddCardCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<DashboardCardDto> Handle(AddCardCommand request, CancellationToken cancellationToken)
    {
        var dashboard = await _context.Dashboards
            .FirstOrDefaultAsync(d => d.DashboardId == request.DashboardId, cancellationToken);

        if (dashboard == null)
            throw new InvalidOperationException($"Dashboard with ID {request.DashboardId} not found");

        var cardId = Guid.NewGuid().ToString();
        var position = new CardPosition(request.Row, request.Column, request.Width, request.Height);

        var card = new DashboardCard(
            cardId,
            request.DashboardId,
            request.CardType,
            request.ConfigurationJson,
            position
        );

        dashboard.AddCard(card, request.AddedBy);
        await _context.SaveChangesAsync(cancellationToken);

        return card.ToDto();
    }
}
