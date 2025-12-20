using MediatR;
using StartupStarter.Api.Features.ApiManagement.Dtos;

namespace StartupStarter.Api.Features.ApiManagement.Commands;

public class RevokeApiKeyCommand : IRequest<ApiKeyDto?>
{
    public string ApiKeyId { get; set; } = string.Empty;
    public string RevokedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
