using MediatR;
using StartupStarter.Api.Features.WebhookManagement.Dtos;

namespace StartupStarter.Api.Features.WebhookManagement.Queries;

public class GetWebhookByIdQuery : IRequest<WebhookDto?>
{
    public string WebhookId { get; set; } = string.Empty;
}
