using MediatR;
using StartupStarter.Api.Features.WebhookManagement.Dtos;

namespace StartupStarter.Api.Features.WebhookManagement.Commands;

public class RegisterWebhookCommand : IRequest<WebhookDto>
{
    public string Url { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> EventTypes { get; set; } = new();
    public string RegisteredBy { get; set; } = string.Empty;
}
