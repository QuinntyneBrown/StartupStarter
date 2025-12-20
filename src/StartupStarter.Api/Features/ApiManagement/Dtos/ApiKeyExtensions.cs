using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Api.Features.ApiManagement.Dtos;

public static class ApiKeyExtensions
{
    public static ApiKeyDto ToDto(this ApiKey apiKey)
    {
        return new ApiKeyDto
        {
            ApiKeyId = apiKey.ApiKeyId,
            AccountId = apiKey.AccountId,
            KeyName = apiKey.KeyName,
            KeyHash = apiKey.KeyHash,
            Permissions = string.Join(",", apiKey.Permissions),
            LastUsedAt = null,
            ExpiresAt = apiKey.ExpiresAt,
            CreatedAt = apiKey.CreatedAt,
            RevokedAt = apiKey.RevokedAt
        };
    }

    public static ApiRequestDto ToDto(this ApiRequest apiRequest)
    {
        return new ApiRequestDto
        {
            RequestId = apiRequest.RequestId,
            ApiKeyId = apiRequest.ApiKeyId,
            Endpoint = apiRequest.Endpoint,
            HttpMethod = apiRequest.Method.ToString(),
            ResponseStatus = apiRequest.ResponseStatusCode,
            RequestedAt = apiRequest.Timestamp
        };
    }
}
