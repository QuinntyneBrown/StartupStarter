using MediatR;
using StartupStarter.Api.Features.ApiManagement.Dtos;

namespace StartupStarter.Api.Features.ApiManagement.Queries;

public class GetApiRequestsByApiKeyIdQuery : IRequest<List<ApiRequestDto>>
{
    public string ApiKeyId { get; set; } = string.Empty;
}
