using MediatR;

namespace StartupStarter.Api.Features.WebhookManagement.Commands;

public class DeleteWebhookCommand : IRequest<bool>
{
    public string WebhookId { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
}
