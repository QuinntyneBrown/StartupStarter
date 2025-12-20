using MediatR;
using StartupStarter.Api.Features.WebhookManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Api.Features.WebhookManagement.Commands;

public class RegisterWebhookCommandHandler : IRequestHandler<RegisterWebhookCommand, WebhookDto>
{
    private readonly IStartupStarterContext _context;

    public RegisterWebhookCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<WebhookDto> Handle(RegisterWebhookCommand request, CancellationToken cancellationToken)
    {
        var webhookId = Guid.NewGuid().ToString();

        var webhook = new Webhook(
            webhookId,
            request.Url,
            request.AccountId,
            request.RegisteredBy,
            request.EventTypes
        );

        _context.Webhooks.Add(webhook);
        await _context.SaveChangesAsync(cancellationToken);

        return webhook.ToDto();
    }
}
