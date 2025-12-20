using MediatR;
using StartupStarter.Api.Features.WebhookManagement.Dtos;

namespace StartupStarter.Api.Features.WebhookManagement.Queries;

public class GetWebhookDeliveriesByWebhookIdQuery : IRequest<List<WebhookDeliveryDto>>
{
    public string WebhookId { get; set; } = string.Empty;
}
