# API Management Models

## Core Aggregate

### ApiKeyAggregate

Located in: `StartupStarter.Core\Model\ApiKeyAggregate\`

#### Entities

**ApiKey.cs** (Aggregate Root)
```csharp
public class ApiKey
{
    public string ApiKeyId { get; private set; }
    public string KeyName { get; private set; }
    public string AccountId { get; private set; }
    public string KeyHash { get; private set; } // Hashed version of the actual key
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokedBy { get; private set; }
    public string RevocationReason { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<string> _permissions = new();
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Methods
    public void Revoke(string revokedBy, string reason);
    public void AddPermission(string permission);
    public void RemovePermission(string permission);
}
```

**ApiRequest.cs**
```csharp
public class ApiRequest
{
    public string RequestId { get; private set; }
    public string Endpoint { get; private set; }
    public HttpMethod Method { get; private set; }
    public string ApiKeyId { get; private set; }
    public string AccountId { get; private set; }
    public string IPAddress { get; private set; }
    public DateTime Timestamp { get; private set; }
    public int ResponseStatusCode { get; private set; }
    public long ResponseTimeMs { get; private set; }
    public bool WasRateLimited { get; private set; }

    public ApiKey ApiKey { get; private set; }
}
```

### WebhookAggregate

Located in: `StartupStarter.Core\Model\WebhookAggregate\`

**Webhook.cs** (Aggregate Root)
```csharp
public class Webhook
{
    public string WebhookId { get; private set; }
    public string Url { get; private set; }
    public string AccountId { get; private set; }
    public string RegisteredBy { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string DeletedBy { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<string> _events = new();
    public IReadOnlyCollection<string> Events => _events.AsReadOnly();

    private readonly List<WebhookDelivery> _deliveries = new();
    public IReadOnlyCollection<WebhookDelivery> Deliveries => _deliveries.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Methods
    public void RegisterEvent(string eventType);
    public void UnregisterEvent(string eventType);
    public void Delete(string deletedBy);
    public void RecordDelivery(string eventType, object payload, int responseStatus, bool success, string failureReason = null);
}
```

**WebhookDelivery.cs**
```csharp
public class WebhookDelivery
{
    public string WebhookDeliveryId { get; private set; }
    public string WebhookId { get; private set; }
    public string EventType { get; private set; }
    public string PayloadJson { get; private set; }
    public int ResponseStatus { get; private set; }
    public bool Success { get; private set; }
    public string FailureReason { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Webhook Webhook { get; private set; }

    public void IncrementRetry();
}
```

#### Enums

**HttpMethod.cs**
```csharp
public enum HttpMethod
{
    GET,
    POST,
    PUT,
    PATCH,
    DELETE
}
```

#### Domain Events

**ApiKeyCreatedEvent.cs**
```csharp
public class ApiKeyCreatedEvent : DomainEvent
{
    public string ApiKeyId { get; set; }
    public string KeyName { get; set; }
    public string AccountId { get; set; }
    public string CreatedBy { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ApiKeyRevokedEvent.cs**
```csharp
public class ApiKeyRevokedEvent : DomainEvent
{
    public string ApiKeyId { get; set; }
    public string AccountId { get; set; }
    public string RevokedBy { get; set; }
    public string Reason { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ApiRequestReceivedEvent.cs**
```csharp
public class ApiRequestReceivedEvent : DomainEvent
{
    public string RequestId { get; set; }
    public string Endpoint { get; set; }
    public HttpMethod Method { get; set; }
    public string ApiKeyId { get; set; }
    public string AccountId { get; set; }
    public string IPAddress { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**ApiRequestRateLimitedEvent.cs**
```csharp
public class ApiRequestRateLimitedEvent : DomainEvent
{
    public string RequestId { get; set; }
    public string Endpoint { get; set; }
    public string ApiKeyId { get; set; }
    public string AccountId { get; set; }
    public string RateLimitTier { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**WebhookRegisteredEvent.cs**
```csharp
public class WebhookRegisteredEvent : DomainEvent
{
    public string WebhookId { get; set; }
    public string Url { get; set; }
    public List<string> Events { get; set; }
    public string AccountId { get; set; }
    public string RegisteredBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**WebhookTriggeredEvent.cs**
```csharp
public class WebhookTriggeredEvent : DomainEvent
{
    public string WebhookId { get; set; }
    public string AccountId { get; set; }
    public string EventType { get; set; }
    public object PayloadSent { get; set; }
    public int ResponseStatus { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**WebhookFailedEvent.cs**
```csharp
public class WebhookFailedEvent : DomainEvent
{
    public string WebhookId { get; set; }
    public string AccountId { get; set; }
    public string EventType { get; set; }
    public string FailureReason { get; set; }
    public int RetryCount { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**WebhookDeletedEvent.cs**
```csharp
public class WebhookDeletedEvent : DomainEvent
{
    public string WebhookId { get; set; }
    public string AccountId { get; set; }
    public string DeletedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Infrastructure

### Entity Configuration

**ApiKeyConfiguration.cs**
Located in: `StartupStarter.Infrastructure\EntityConfigurations\`

```csharp
public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");
        builder.HasKey(k => k.ApiKeyId);

        builder.Property(k => k.ApiKeyId).IsRequired();
        builder.Property(k => k.KeyName).IsRequired().HasMaxLength(200);
        builder.Property(k => k.AccountId).IsRequired();
        builder.Property(k => k.KeyHash).IsRequired();

        builder.Ignore(k => k.DomainEvents);
    }
}
```

**WebhookConfiguration.cs**
```csharp
public class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.ToTable("Webhooks");
        builder.HasKey(w => w.WebhookId);

        builder.Property(w => w.Url).IsRequired().HasMaxLength(500);
        builder.Property(w => w.AccountId).IsRequired();

        builder.HasMany(w => w.Deliveries)
            .WithOne(d => d.Webhook)
            .HasForeignKey(d => d.WebhookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(w => w.DomainEvents);
    }
}
```

**ApiRequestConfiguration.cs**
```csharp
public class ApiRequestConfiguration : IEntityTypeConfiguration<ApiRequest>
{
    public void Configure(EntityTypeBuilder<ApiRequest> builder)
    {
        builder.ToTable("ApiRequests");
        builder.HasKey(r => r.RequestId);

        builder.Property(r => r.Endpoint).IsRequired().HasMaxLength(500);
        builder.Property(r => r.ApiKeyId).IsRequired();
        builder.Property(r => r.AccountId).IsRequired();
        builder.Property(r => r.IPAddress).HasMaxLength(45);

        builder.HasOne(r => r.ApiKey)
            .WithMany()
            .HasForeignKey(r => r.ApiKeyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## API Layer

### DTOs

**ApiKeyDto.cs**
Located in: `StartupStarter.Api\Dtos\`

```csharp
public class ApiKeyDto
{
    public string ApiKeyId { get; set; }
    public string KeyName { get; set; }
    public string AccountId { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
```

**CreateApiKeyDto.cs**
```csharp
public class CreateApiKeyDto
{
    public string KeyName { get; set; }
    public string AccountId { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
```

**WebhookDto.cs**
```csharp
public class WebhookDto
{
    public string WebhookId { get; set; }
    public string Url { get; set; }
    public string AccountId { get; set; }
    public List<string> Events { get; set; }
    public DateTime RegisteredAt { get; set; }
    public bool IsActive { get; set; }
}
```

**CreateWebhookDto.cs**
```csharp
public class CreateWebhookDto
{
    public string Url { get; set; }
    public string AccountId { get; set; }
    public List<string> Events { get; set; }
}
```

### Extension Methods

**ApiKeyExtensions.cs**
Located in: `StartupStarter.Api\Extensions\`

```csharp
public static class ApiKeyExtensions
{
    public static ApiKeyDto ToDto(this ApiKey apiKey)
    {
        return new ApiKeyDto
        {
            ApiKeyId = apiKey.ApiKeyId,
            KeyName = apiKey.KeyName,
            AccountId = apiKey.AccountId,
            Permissions = apiKey.Permissions.ToList(),
            CreatedAt = apiKey.CreatedAt,
            ExpiresAt = apiKey.ExpiresAt,
            IsActive = apiKey.IsActive
        };
    }
}
```

**WebhookExtensions.cs**
```csharp
public static class WebhookExtensions
{
    public static WebhookDto ToDto(this Webhook webhook)
    {
        return new WebhookDto
        {
            WebhookId = webhook.WebhookId,
            Url = webhook.Url,
            AccountId = webhook.AccountId,
            Events = webhook.Events.ToList(),
            RegisteredAt = webhook.RegisteredAt,
            IsActive = webhook.IsActive
        };
    }
}
```

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\Api\Commands\`

**CreateApiKeyCommand.cs**
```csharp
public class CreateApiKeyCommand : IRequest<ApiKeyDto>
{
    public string KeyName { get; set; }
    public string AccountId { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string CreatedBy { get; set; }
}
```

**RevokeApiKeyCommand.cs**
```csharp
public class RevokeApiKeyCommand : IRequest<bool>
{
    public string ApiKeyId { get; set; }
    public string RevokedBy { get; set; }
    public string Reason { get; set; }
}
```

**RegisterWebhookCommand.cs**
```csharp
public class RegisterWebhookCommand : IRequest<WebhookDto>
{
    public string Url { get; set; }
    public List<string> Events { get; set; }
    public string AccountId { get; set; }
    public string RegisteredBy { get; set; }
}
```

**DeleteWebhookCommand.cs**
```csharp
public class DeleteWebhookCommand : IRequest<bool>
{
    public string WebhookId { get; set; }
    public string DeletedBy { get; set; }
}
```

**RecordApiRequestCommand.cs**
```csharp
public class RecordApiRequestCommand : IRequest<bool>
{
    public string Endpoint { get; set; }
    public HttpMethod Method { get; set; }
    public string ApiKeyId { get; set; }
    public string IPAddress { get; set; }
}
```

### Queries (MediatR)

Located in: `StartupStarter.Api\Features\Api\Queries\`

**GetApiKeysByAccountQuery.cs**
```csharp
public class GetApiKeysByAccountQuery : IRequest<List<ApiKeyDto>>
{
    public string AccountId { get; set; }
}
```

**GetWebhooksByAccountQuery.cs**
```csharp
public class GetWebhooksByAccountQuery : IRequest<List<WebhookDto>>
{
    public string AccountId { get; set; }
}
```

**GetApiRequestHistoryQuery.cs**
```csharp
public class GetApiRequestHistoryQuery : IRequest<List<ApiRequest>>
{
    public string AccountId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
```
