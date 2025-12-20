using StartupStarter.Core.Model.ApiKeyAggregate.Enums;

namespace StartupStarter.Core.Model.ApiKeyAggregate.Entities;

public class ApiRequest
{
    public string RequestId { get; private set; }
    public string Endpoint { get; private set; }
    public ApiHttpMethod Method { get; private set; }
    public string ApiKeyId { get; private set; }
    public string AccountId { get; private set; }
    public string IPAddress { get; private set; }
    public DateTime Timestamp { get; private set; }
    public int ResponseStatusCode { get; private set; }
    public long ResponseTimeMs { get; private set; }
    public bool WasRateLimited { get; private set; }

    public ApiKey ApiKey { get; private set; } = null!;

    // EF Core constructor
    private ApiRequest()
    {
        RequestId = string.Empty;
        Endpoint = string.Empty;
        ApiKeyId = string.Empty;
        AccountId = string.Empty;
        IPAddress = string.Empty;
    }

    public ApiRequest(string requestId, string endpoint, ApiHttpMethod method, string apiKeyId,
        string accountId, string ipAddress, int responseStatusCode, long responseTimeMs, bool wasRateLimited)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("Request ID cannot be empty", nameof(requestId));
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Endpoint cannot be empty", nameof(endpoint));
        if (string.IsNullOrWhiteSpace(apiKeyId))
            throw new ArgumentException("API Key ID cannot be empty", nameof(apiKeyId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));

        RequestId = requestId;
        Endpoint = endpoint;
        Method = method;
        ApiKeyId = apiKeyId;
        AccountId = accountId;
        IPAddress = ipAddress ?? string.Empty;
        Timestamp = DateTime.UtcNow;
        ResponseStatusCode = responseStatusCode;
        ResponseTimeMs = responseTimeMs;
        WasRateLimited = wasRateLimited;
    }
}
