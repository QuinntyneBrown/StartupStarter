using MediatR;
using StartupStarter.Api.Features.ApiManagement.Dtos;

namespace StartupStarter.Api.Features.ApiManagement.Commands;

public class CreateApiKeyCommand : IRequest<ApiKeyDto>
{
    public string KeyName { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
