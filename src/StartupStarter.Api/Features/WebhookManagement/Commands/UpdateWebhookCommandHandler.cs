using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.WebhookManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WebhookManagement.Commands;

public class UpdateWebhookCommandHandler : IRequestHandler<UpdateWebhookCommand, WebhookDto?>
{
    private readonly IStartupStarterContext _context;

    public UpdateWebhookCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WebhookDto?> Handle(UpdateWebhookCommand request, CancellationToken cancellationToken)
    {
        var webhook = await _context.Webhooks
            .FirstOrDefaultAsync(w => w.WebhookId == request.WebhookId, cancellationToken);

        if (webhook == null)
            return null;

        // Update event types if provided
        if (request.EventTypes != null)
        {
            // Clear existing events and add new ones
            foreach (var eventType in webhook.Events.ToList())
            {
                webhook.UnregisterEvent(eventType);
            }

            foreach (var eventType in request.EventTypes)
            {
                webhook.RegisterEvent(eventType);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return webhook.ToDto();
    }
}
