using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WebhookManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WebhookManagement.Queries;

public class GetWebhookDeliveriesByWebhookIdQueryHandler : IRequestHandler<GetWebhookDeliveriesByWebhookIdQuery, List<WebhookDeliveryDto>>
{
    private readonly IStartupStarterContext _context;

    public GetWebhookDeliveriesByWebhookIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<List<WebhookDeliveryDto>> Handle(GetWebhookDeliveriesByWebhookIdQuery request, CancellationToken cancellationToken)
    {
        var deliveries = await _context.WebhookDeliveries
            .Where(d => d.WebhookId == request.WebhookId)
            .OrderByDescending(d => d.Timestamp)
            .ToListAsync(cancellationToken);

        return deliveries.Select(d => d.ToDto()).ToList();
    }
}
