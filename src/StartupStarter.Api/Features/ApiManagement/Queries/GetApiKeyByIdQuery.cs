using MediatR;
using StartupStarter.Api.Features.ApiManagement.Dtos;

namespace StartupStarter.Api.Features.ApiManagement.Queries;

public class GetApiKeyByIdQuery : IRequest<ApiKeyDto?>
{
    public string ApiKeyId { get; set; } = string.Empty;
}
