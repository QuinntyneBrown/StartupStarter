using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.WebhookManagement.Commands;

public class DeleteWebhookCommandHandler : IRequestHandler<DeleteWebhookCommand, bool>
{
    private readonly IStartupStarterContext _context;

    public DeleteWebhookCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteWebhookCommand request, CancellationToken cancellationToken)
    {
        var webhook = await _context.Webhooks
            .FirstOrDefaultAsync(w => w.WebhookId == request.WebhookId, cancellationToken);

        if (webhook == null)
            return false;

        webhook.Delete(request.DeletedBy);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
