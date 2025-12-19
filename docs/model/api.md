# API Management Domain Models

This document defines the C# models needed to implement the API management events following Clean Architecture principles with MediatR.

## Domain Entities

### ApiKey (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class ApiKey : AggregateRoot
    {
        public Guid ApiKeyId { get; private set; }
        public string KeyName { get; private set; }
        public string HashedKey { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid CreatedBy { get; private set; }
        public List<string> Permissions { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ApiKeyStatus Status { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public Guid? RevokedBy { get; private set; }
        public string? RevocationReason { get; private set; }

        // Navigation properties
        public virtual Account Account { get; private set; }
        public virtual User Creator { get; private set; }
        public virtual ICollection<ApiRequest> Requests { get; private set; } = new List<ApiRequest>();

        private ApiKey() { } // EF Core

        public static ApiKey Create(
            string keyName,
            string hashedKey,
            Guid accountId,
            Guid createdBy,
            List<string> permissions,
            DateTime? expiresAt = null)
        {
            var apiKey = new ApiKey
            {
                ApiKeyId = Guid.NewGuid(),
                KeyName = keyName,
                HashedKey = hashedKey,
                AccountId = accountId,
                CreatedBy = createdBy,
                Permissions = permissions ?? new List<string>(),
                ExpiresAt = expiresAt,
                Status = ApiKeyStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            apiKey.AddDomainEvent(new ApiKeyCreatedDomainEvent(
                apiKey.ApiKeyId,
                apiKey.KeyName,
                apiKey.AccountId,
                apiKey.CreatedBy,
                apiKey.Permissions,
                apiKey.ExpiresAt));

            return apiKey;
        }

        public void Revoke(Guid revokedBy, string reason)
        {
            if (Status == ApiKeyStatus.Revoked)
                throw new InvalidOperationException("API key is already revoked.");

            Status = ApiKeyStatus.Revoked;
            RevokedAt = DateTime.UtcNow;
            RevokedBy = revokedBy;
            RevocationReason = reason;

            AddDomainEvent(new ApiKeyRevokedDomainEvent(
                ApiKeyId,
                AccountId,
                revokedBy,
                reason));
        }

        public bool IsValid()
        {
            if (Status != ApiKeyStatus.Active)
                return false;

            if (ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow)
            {
                Status = ApiKeyStatus.Expired;
                return false;
            }

            return true;
        }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission) || Permissions.Contains("*");
        }
    }
}
```

### Webhook (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class Webhook : AggregateRoot
    {
        public Guid WebhookId { get; private set; }
        public string Url { get; private set; }
        public List<string> Events { get; private set; }
        public Guid AccountId { get; private set; }
        public Guid RegisteredBy { get; private set; }
        public WebhookStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedBy { get; private set; }
        public bool IsDeleted { get; private set; }
        public int FailureCount { get; private set; }
        public DateTime? LastTriggeredAt { get; private set; }

        // Navigation properties
        public virtual Account Account { get; private set; }
        public virtual User Registrant { get; private set; }
        public virtual ICollection<WebhookDelivery> Deliveries { get; private set; } = new List<WebhookDelivery>();

        private Webhook() { } // EF Core

        public static Webhook Register(
            string url,
            List<string> events,
            Guid accountId,
            Guid registeredBy)
        {
            var webhook = new Webhook
            {
                WebhookId = Guid.NewGuid(),
                Url = url,
                Events = events ?? new List<string>(),
                AccountId = accountId,
                RegisteredBy = registeredBy,
                Status = WebhookStatus.Active,
                FailureCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            webhook.AddDomainEvent(new WebhookRegisteredDomainEvent(
                webhook.WebhookId,
                webhook.Url,
                webhook.Events,
                webhook.AccountId,
                webhook.RegisteredBy));

            return webhook;
        }

        public void RecordSuccessfulDelivery(string eventType, object payload, int responseStatus)
        {
            LastTriggeredAt = DateTime.UtcNow;
            FailureCount = 0; // Reset failure count on success

            if (Status == WebhookStatus.Failed)
                Status = WebhookStatus.Active;

            AddDomainEvent(new WebhookTriggeredDomainEvent(
                WebhookId,
                AccountId,
                eventType,
                payload,
                responseStatus));
        }

        public void RecordFailedDelivery(string eventType, string failureReason, int retryCount)
        {
            LastTriggeredAt = DateTime.UtcNow;
            FailureCount++;

            // Disable webhook after 5 consecutive failures
            if (FailureCount >= 5)
                Status = WebhookStatus.Failed;

            AddDomainEvent(new WebhookFailedDomainEvent(
                WebhookId,
                AccountId,
                eventType,
                failureReason,
                retryCount));
        }

        public void Delete(Guid deletedBy)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Webhook is already deleted.");

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            Status = WebhookStatus.Inactive;

            AddDomainEvent(new WebhookDeletedDomainEvent(
                WebhookId,
                AccountId,
                deletedBy));
        }

        public void UpdateUrl(string newUrl)
        {
            Url = newUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEvents(List<string> newEvents)
        {
            Events = newEvents ?? new List<string>();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            Status = WebhookStatus.Active;
            FailureCount = 0;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = WebhookStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool SubscribesToEvent(string eventType)
        {
            return Events.Contains(eventType) || Events.Contains("*");
        }
    }
}
```

### WebhookDelivery (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class WebhookDelivery
    {
        public Guid DeliveryId { get; private set; }
        public Guid WebhookId { get; private set; }
        public string EventType { get; private set; }
        public string PayloadSent { get; private set; } // JSON string
        public int? ResponseStatus { get; private set; }
        public string? FailureReason { get; private set; }
        public int RetryCount { get; private set; }
        public bool IsSuccess { get; private set; }
        public DateTime AttemptedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public TimeSpan? ResponseTime { get; private set; }

        // Navigation
        public virtual Webhook Webhook { get; private set; }

        private WebhookDelivery() { } // EF Core

        public static WebhookDelivery CreateAttempt(
            Guid webhookId,
            string eventType,
            string payloadSent,
            int retryCount = 0)
        {
            return new WebhookDelivery
            {
                DeliveryId = Guid.NewGuid(),
                WebhookId = webhookId,
                EventType = eventType,
                PayloadSent = payloadSent,
                RetryCount = retryCount,
                IsSuccess = false,
                AttemptedAt = DateTime.UtcNow
            };
        }

        public void MarkSuccess(int responseStatus, TimeSpan responseTime)
        {
            IsSuccess = true;
            ResponseStatus = responseStatus;
            ResponseTime = responseTime;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkFailure(string failureReason, int? responseStatus = null)
        {
            IsSuccess = false;
            FailureReason = failureReason;
            ResponseStatus = responseStatus;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
```

### ApiRequest (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class ApiRequest : AggregateRoot
    {
        public Guid RequestId { get; private set; }
        public string Endpoint { get; private set; }
        public HttpMethod Method { get; private set; }
        public Guid ApiKeyId { get; private set; }
        public Guid AccountId { get; private set; }
        public string IPAddress { get; private set; }
        public int? ResponseStatus { get; private set; }
        public bool WasRateLimited { get; private set; }
        public RateLimitTier? RateLimitTier { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan? ResponseTime { get; private set; }
        public long? RequestSize { get; private set; }
        public long? ResponseSize { get; private set; }
        public string? UserAgent { get; private set; }

        // Navigation properties
        public virtual ApiKey ApiKey { get; private set; }
        public virtual Account Account { get; private set; }

        private ApiRequest() { } // EF Core

        public static ApiRequest Create(
            string endpoint,
            HttpMethod method,
            Guid apiKeyId,
            Guid accountId,
            string ipAddress,
            string? userAgent = null)
        {
            var request = new ApiRequest
            {
                RequestId = Guid.NewGuid(),
                Endpoint = endpoint,
                Method = method,
                ApiKeyId = apiKeyId,
                AccountId = accountId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                WasRateLimited = false,
                Timestamp = DateTime.UtcNow
            };

            request.AddDomainEvent(new ApiRequestReceivedDomainEvent(
                request.RequestId,
                request.Endpoint,
                request.Method,
                request.ApiKeyId,
                request.AccountId,
                request.IPAddress));

            return request;
        }

        public void MarkAsRateLimited(RateLimitTier rateLimitTier)
        {
            WasRateLimited = true;
            RateLimitTier = rateLimitTier;
            ResponseStatus = 429; // Too Many Requests

            AddDomainEvent(new ApiRequestRateLimitedDomainEvent(
                RequestId,
                Endpoint,
                ApiKeyId,
                AccountId,
                rateLimitTier));
        }

        public void Complete(int responseStatus, TimeSpan responseTime, long? requestSize = null, long? responseSize = null)
        {
            ResponseStatus = responseStatus;
            ResponseTime = responseTime;
            RequestSize = requestSize;
            ResponseSize = responseSize;
        }
    }
}
```

## Enumerations

### HttpMethod

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum HttpMethod
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        PATCH = 3,
        DELETE = 4,
        HEAD = 5,
        OPTIONS = 6
    }
}
```

### RateLimitTier

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum RateLimitTier
    {
        Free = 0,      // 100 requests/hour
        Basic = 1,     // 1,000 requests/hour
        Pro = 2,       // 10,000 requests/hour
        Enterprise = 3, // 100,000 requests/hour
        Unlimited = 4  // No rate limit
    }
}
```

### WebhookStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum WebhookStatus
    {
        Active = 0,
        Inactive = 1,
        Failed = 2
    }
}
```

### ApiKeyStatus

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ApiKeyStatus
    {
        Active = 0,
        Revoked = 1,
        Expired = 2
    }
}
```

## Domain Events

### ApiKeyCreatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ApiKeyCreatedDomainEvent(
        Guid ApiKeyId,
        string KeyName,
        Guid AccountId,
        Guid CreatedBy,
        List<string> Permissions,
        DateTime? ExpiresAt) : DomainEvent;
}
```

### ApiKeyRevokedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ApiKeyRevokedDomainEvent(
        Guid ApiKeyId,
        Guid AccountId,
        Guid RevokedBy,
        string Reason) : DomainEvent;
}
```

### ApiRequestReceivedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ApiRequestReceivedDomainEvent(
        Guid RequestId,
        string Endpoint,
        HttpMethod Method,
        Guid ApiKeyId,
        Guid AccountId,
        string IPAddress) : DomainEvent;
}
```

### ApiRequestRateLimitedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record ApiRequestRateLimitedDomainEvent(
        Guid RequestId,
        string Endpoint,
        Guid ApiKeyId,
        Guid AccountId,
        RateLimitTier RateLimitTier) : DomainEvent;
}
```

### WebhookRegisteredDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record WebhookRegisteredDomainEvent(
        Guid WebhookId,
        string Url,
        List<string> Events,
        Guid AccountId,
        Guid RegisteredBy) : DomainEvent;
}
```

### WebhookTriggeredDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record WebhookTriggeredDomainEvent(
        Guid WebhookId,
        Guid AccountId,
        string EventType,
        object PayloadSent,
        int ResponseStatus) : DomainEvent;
}
```

### WebhookFailedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record WebhookFailedDomainEvent(
        Guid WebhookId,
        Guid AccountId,
        string EventType,
        string FailureReason,
        int RetryCount) : DomainEvent;
}
```

### WebhookDeletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record WebhookDeletedDomainEvent(
        Guid WebhookId,
        Guid AccountId,
        Guid DeletedBy) : DomainEvent;
}
```

## MediatR Commands

### CreateApiKeyCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record CreateApiKeyCommand(
        string KeyName,
        Guid AccountId,
        Guid CreatedBy,
        List<string> Permissions,
        DateTime? ExpiresAt = null) : IRequest<CreateApiKeyResponse>;

    public record CreateApiKeyResponse(
        Guid ApiKeyId,
        string ApiKey, // Plain text key returned only once
        DateTime CreatedAt,
        DateTime? ExpiresAt);
}
```

### RevokeApiKeyCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record RevokeApiKeyCommand(
        Guid ApiKeyId,
        Guid RevokedBy,
        string Reason) : IRequest<Unit>;
}
```

### RegisterWebhookCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record RegisterWebhookCommand(
        string Url,
        List<string> Events,
        Guid AccountId,
        Guid RegisteredBy) : IRequest<RegisterWebhookResponse>;

    public record RegisterWebhookResponse(
        Guid WebhookId,
        WebhookStatus Status,
        DateTime CreatedAt);
}
```

### DeleteWebhookCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record DeleteWebhookCommand(
        Guid WebhookId,
        Guid DeletedBy) : IRequest<Unit>;
}
```

### UpdateWebhookCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record UpdateWebhookCommand(
        Guid WebhookId,
        string? Url = null,
        List<string>? Events = null) : IRequest<Unit>;
}
```

### TriggerWebhookCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record TriggerWebhookCommand(
        Guid WebhookId,
        string EventType,
        object Payload) : IRequest<TriggerWebhookResponse>;

    public record TriggerWebhookResponse(
        bool Success,
        int? ResponseStatus,
        string? FailureReason);
}
```

### LogApiRequestCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record LogApiRequestCommand(
        string Endpoint,
        HttpMethod Method,
        Guid ApiKeyId,
        Guid AccountId,
        string IPAddress,
        string? UserAgent = null) : IRequest<LogApiRequestResponse>;

    public record LogApiRequestResponse(
        Guid RequestId,
        DateTime Timestamp);
}
```

### CompleteApiRequestCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record CompleteApiRequestCommand(
        Guid RequestId,
        int ResponseStatus,
        TimeSpan ResponseTime,
        long? RequestSize = null,
        long? ResponseSize = null) : IRequest<Unit>;
}
```

### RecordRateLimitCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record RecordRateLimitCommand(
        Guid RequestId,
        RateLimitTier RateLimitTier) : IRequest<Unit>;
}
```

### ActivateWebhookCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record ActivateWebhookCommand(Guid WebhookId) : IRequest<Unit>;
}
```

### DeactivateWebhookCommand

```csharp
namespace StartupStarter.Application.Api.Commands
{
    public record DeactivateWebhookCommand(Guid WebhookId) : IRequest<Unit>;
}
```

## MediatR Queries

### GetApiKeyByIdQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetApiKeyByIdQuery(Guid ApiKeyId) : IRequest<ApiKeyDto?>;
}
```

### GetApiKeysByAccountQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetApiKeysByAccountQuery(
        Guid AccountId,
        ApiKeyStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<ApiKeyDto>>;
}
```

### ValidateApiKeyQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record ValidateApiKeyQuery(string ApiKey) : IRequest<ApiKeyValidationResult>;

    public record ApiKeyValidationResult(
        bool IsValid,
        Guid? ApiKeyId,
        Guid? AccountId,
        List<string>? Permissions,
        string? InvalidReason);
}
```

### GetWebhookByIdQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetWebhookByIdQuery(Guid WebhookId) : IRequest<WebhookDto?>;
}
```

### GetWebhooksByAccountQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetWebhooksByAccountQuery(
        Guid AccountId,
        WebhookStatus? Status = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<WebhookDto>>;
}
```

### GetWebhookDeliveriesQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetWebhookDeliveriesQuery(
        Guid WebhookId,
        bool? SuccessOnly = null,
        int PageNumber = 1,
        int PageSize = 50) : IRequest<PaginatedList<WebhookDeliveryDto>>;
}
```

### GetApiRequestLogsQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetApiRequestLogsQuery(
        Guid? AccountId = null,
        Guid? ApiKeyId = null,
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        bool? RateLimitedOnly = null,
        int PageNumber = 1,
        int PageSize = 100) : IRequest<PaginatedList<ApiRequestDto>>;
}
```

### GetApiUsageStatisticsQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetApiUsageStatisticsQuery(
        Guid AccountId,
        DateTime StartDate,
        DateTime EndDate) : IRequest<ApiUsageStatisticsDto>;
}
```

### GetWebhooksByEventTypeQuery

```csharp
namespace StartupStarter.Application.Api.Queries
{
    public record GetWebhooksByEventTypeQuery(
        Guid AccountId,
        string EventType) : IRequest<List<WebhookDto>>;
}
```

## DTOs

### ApiKeyDto

```csharp
namespace StartupStarter.Application.Api.DTOs
{
    public record ApiKeyDto(
        Guid ApiKeyId,
        string KeyName,
        string MaskedKey, // e.g., "sk_...abc123"
        Guid AccountId,
        string AccountName,
        Guid CreatedBy,
        string CreatedByName,
        List<string> Permissions,
        ApiKeyStatus Status,
        DateTime? ExpiresAt,
        DateTime CreatedAt,
        DateTime? RevokedAt,
        Guid? RevokedBy,
        string? RevokedByName,
        string? RevocationReason);
}
```

### WebhookDto

```csharp
namespace StartupStarter.Application.Api.DTOs
{
    public record WebhookDto(
        Guid WebhookId,
        string Url,
        List<string> Events,
        Guid AccountId,
        string AccountName,
        Guid RegisteredBy,
        string RegisteredByName,
        WebhookStatus Status,
        int FailureCount,
        DateTime? LastTriggeredAt,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
```

### WebhookDeliveryDto

```csharp
namespace StartupStarter.Application.Api.DTOs
{
    public record WebhookDeliveryDto(
        Guid DeliveryId,
        Guid WebhookId,
        string EventType,
        string PayloadSent,
        int? ResponseStatus,
        string? FailureReason,
        int RetryCount,
        bool IsSuccess,
        DateTime AttemptedAt,
        DateTime? CompletedAt,
        TimeSpan? ResponseTime);
}
```

### ApiRequestDto

```csharp
namespace StartupStarter.Application.Api.DTOs
{
    public record ApiRequestDto(
        Guid RequestId,
        string Endpoint,
        HttpMethod Method,
        Guid ApiKeyId,
        string ApiKeyName,
        Guid AccountId,
        string AccountName,
        string IPAddress,
        int? ResponseStatus,
        bool WasRateLimited,
        RateLimitTier? RateLimitTier,
        DateTime Timestamp,
        TimeSpan? ResponseTime,
        long? RequestSize,
        long? ResponseSize,
        string? UserAgent);
}
```

### ApiUsageStatisticsDto

```csharp
namespace StartupStarter.Application.Api.DTOs
{
    public record ApiUsageStatisticsDto(
        Guid AccountId,
        DateTime StartDate,
        DateTime EndDate,
        long TotalRequests,
        long SuccessfulRequests,
        long FailedRequests,
        long RateLimitedRequests,
        Dictionary<string, long> RequestsByEndpoint,
        Dictionary<HttpMethod, long> RequestsByMethod,
        Dictionary<int, long> RequestsByStatusCode,
        long TotalDataTransferred,
        double AverageResponseTime);
}
```

## Base Classes

### AggregateRoot

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
```

### DomainEvent

```csharp
namespace StartupStarter.Domain.Common
{
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
```

## Repository Interfaces

### IApiKeyRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IApiKeyRepository
    {
        Task<ApiKey?> GetByIdAsync(Guid apiKeyId, CancellationToken cancellationToken = default);
        Task<ApiKey?> GetByHashedKeyAsync(string hashedKey, CancellationToken cancellationToken = default);
        Task<List<ApiKey>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<PaginatedList<ApiKey>> GetPagedByAccountIdAsync(
            Guid accountId,
            ApiKeyStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<ApiKey>> GetActiveKeysByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<List<ApiKey>> GetExpiringKeysAsync(DateTime beforeDate, CancellationToken cancellationToken = default);
        Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
        Task UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
        Task DeleteAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IWebhookRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IWebhookRepository
    {
        Task<Webhook?> GetByIdAsync(Guid webhookId, CancellationToken cancellationToken = default);
        Task<List<Webhook>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
        Task<PaginatedList<Webhook>> GetPagedByAccountIdAsync(
            Guid accountId,
            WebhookStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<Webhook>> GetActiveWebhooksForEventAsync(Guid accountId, string eventType, CancellationToken cancellationToken = default);
        Task<List<Webhook>> GetFailedWebhooksAsync(CancellationToken cancellationToken = default);
        Task<PaginatedList<WebhookDelivery>> GetDeliveriesAsync(
            Guid webhookId,
            bool? successOnly,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task AddAsync(Webhook webhook, CancellationToken cancellationToken = default);
        Task AddDeliveryAsync(WebhookDelivery delivery, CancellationToken cancellationToken = default);
        Task UpdateAsync(Webhook webhook, CancellationToken cancellationToken = default);
        Task DeleteAsync(Webhook webhook, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IApiRequestRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IApiRequestRepository
    {
        Task<ApiRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
        Task<PaginatedList<ApiRequest>> GetPagedAsync(
            Guid? accountId,
            Guid? apiKeyId,
            DateTime? startDate,
            DateTime? endDate,
            bool? rateLimitedOnly,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<List<ApiRequest>> GetByAccountIdAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<List<ApiRequest>> GetByApiKeyIdAsync(
            Guid apiKeyId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<long> GetRequestCountAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<long> GetRateLimitedCountAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<Dictionary<string, long>> GetRequestsByEndpointAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<Dictionary<HttpMethod, long>> GetRequestsByMethodAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<Dictionary<int, long>> GetRequestsByStatusCodeAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<long> GetTotalDataTransferredAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task<double> GetAverageResponseTimeAsync(
            Guid accountId,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);
        Task AddAsync(ApiRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(ApiRequest request, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

## Supporting Types

### PaginatedList

```csharp
namespace StartupStarter.Application.Common.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
```

## Usage Examples

### Creating an API Key

```csharp
// In a Command Handler
public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResponse>
{
    private readonly IApiKeyRepository _repository;
    private readonly ISecurityService _securityService;

    public async Task<CreateApiKeyResponse> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        // Generate a cryptographically secure API key
        var plainTextKey = _securityService.GenerateApiKey();
        var hashedKey = _securityService.HashApiKey(plainTextKey);

        // Create the API key entity
        var apiKey = ApiKey.Create(
            request.KeyName,
            hashedKey,
            request.AccountId,
            request.CreatedBy,
            request.Permissions,
            request.ExpiresAt);

        await _repository.AddAsync(apiKey, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CreateApiKeyResponse(
            apiKey.ApiKeyId,
            plainTextKey, // Return plain text only once
            apiKey.CreatedAt,
            apiKey.ExpiresAt);
    }
}
```

### Registering a Webhook

```csharp
// In a Command Handler
public class RegisterWebhookCommandHandler : IRequestHandler<RegisterWebhookCommand, RegisterWebhookResponse>
{
    private readonly IWebhookRepository _repository;

    public async Task<RegisterWebhookResponse> Handle(RegisterWebhookCommand request, CancellationToken cancellationToken)
    {
        var webhook = Webhook.Register(
            request.Url,
            request.Events,
            request.AccountId,
            request.RegisteredBy);

        await _repository.AddAsync(webhook, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new RegisterWebhookResponse(
            webhook.WebhookId,
            webhook.Status,
            webhook.CreatedAt);
    }
}
```

### Logging an API Request

```csharp
// In a Command Handler
public class LogApiRequestCommandHandler : IRequestHandler<LogApiRequestCommand, LogApiRequestResponse>
{
    private readonly IApiRequestRepository _repository;

    public async Task<LogApiRequestResponse> Handle(LogApiRequestCommand request, CancellationToken cancellationToken)
    {
        var apiRequest = ApiRequest.Create(
            request.Endpoint,
            request.Method,
            request.ApiKeyId,
            request.AccountId,
            request.IPAddress,
            request.UserAgent);

        await _repository.AddAsync(apiRequest, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new LogApiRequestResponse(
            apiRequest.RequestId,
            apiRequest.Timestamp);
    }
}
```

## Architecture Notes

### Clean Architecture Layers

1. **Domain Layer** (`StartupStarter.Domain`)
   - Entities (ApiKey, Webhook, ApiRequest, WebhookDelivery)
   - Enums (HttpMethod, RateLimitTier, WebhookStatus, ApiKeyStatus)
   - Domain Events
   - Common base classes

2. **Application Layer** (`StartupStarter.Application`)
   - MediatR Commands and Queries
   - DTOs
   - Repository Interfaces
   - Command/Query Handlers
   - Domain Event Handlers

3. **Infrastructure Layer** (`StartupStarter.Infrastructure`)
   - EF Core Implementations of Repositories
   - Database Context
   - External Service Integrations (Webhook delivery, etc.)

4. **Presentation Layer** (`StartupStarter.API`)
   - API Controllers
   - Middleware (Authentication, Rate Limiting)
   - Request/Response models

### Best Practices Implemented

1. **Encapsulation**: All entity properties use private setters
2. **Factory Methods**: Static `Create` methods for entity instantiation
3. **Domain Events**: Rich domain events for all state changes
4. **Immutability**: Records for events, commands, queries, and DTOs
5. **Validation**: Domain logic validates business rules
6. **Separation of Concerns**: Clear boundaries between layers
7. **CQRS Pattern**: Separate commands and queries using MediatR
8. **Repository Pattern**: Abstract data access behind interfaces
9. **Value Objects**: Lists and complex types properly encapsulated
10. **Aggregate Roots**: Clear boundaries for consistency

### Security Considerations

1. **API Key Storage**: Store only hashed keys, never plain text
2. **Key Generation**: Use cryptographically secure random generation
3. **Key Masking**: Display only masked versions in responses
4. **Webhook Verification**: Implement signature verification for webhooks
5. **Rate Limiting**: Enforce limits based on tier
6. **Audit Trail**: Log all API requests and key operations
7. **Permission Checking**: Validate permissions before operations
8. **IP Whitelisting**: Optional IP restriction per API key
