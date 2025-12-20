using MediatR;
using StartupStarter.Api.Features.WebhookManagement.Dtos;

namespace StartupStarter.Api.Features.WebhookManagement.Commands;

public class UpdateWebhookCommand : IRequest<WebhookDto?>
{
    public string WebhookId { get; set; } = string.Empty;
    public string? Url { get; set; }
    public List<string>? EventTypes { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}
