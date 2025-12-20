using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WebhookManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WebhookManagement.Queries;

public class GetWebhookByIdQueryHandler : IRequestHandler<GetWebhookByIdQuery, WebhookDto?>
{
    private readonly IStartupStarterContext _context;

    public GetWebhookByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WebhookDto?> Handle(GetWebhookByIdQuery request, CancellationToken cancellationToken)
    {
        var webhook = await _context.Webhooks
            .FirstOrDefaultAsync(w => w.WebhookId == request.WebhookId, cancellationToken);

        return webhook?.ToDto();
    }
}
