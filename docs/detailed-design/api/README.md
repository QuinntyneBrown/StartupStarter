# API Management - Detailed Design

This folder contains the detailed design documentation for the API Management feature implementation using Clean Architecture, MediatR, and Domain-Driven Design principles.

## Overview

The API Management system provides comprehensive API key management, webhook delivery, rate limiting, and API usage tracking capabilities. The implementation follows Clean Architecture principles with clear separation of concerns across layers, enabling secure and scalable API access control for multi-tenant SaaS applications.

## Architecture Documents

### 1. Domain Model (`domain-model.puml`)

**PlantUML class diagram** showing:
- **ApiKey** aggregate root with security and lifecycle management
- **Webhook** aggregate root for webhook configuration and delivery tracking
- **ApiRequest** aggregate root for API usage analytics
- **WebhookDelivery** entity for delivery attempt tracking
- Domain events raised by aggregates
- Value objects (ApiKeyStatus, WebhookStatus, RateLimitTier, HttpMethod)
- Relationships between entities and external references

**Key Design Patterns:**
- Aggregate Root pattern (DDD)
- Domain Events pattern
- Value Objects for type safety
- Repository pattern for data access

### 2. Sequence Diagrams (`sequence-diagrams.puml`)

**Comprehensive flow diagrams** for:
- **Create API Key**: Full MediatR command flow with secure key generation, hashing, and storage
- **Validate API Key & Log Request**: Authentication middleware with request logging and analytics
- **Rate Limiting**: Redis-based rate limit enforcement with tier-based limits
- **Register Webhook**: Webhook registration with endpoint validation and verification
- **Trigger & Deliver Webhook**: Async webhook delivery with retry logic and exponential backoff

**Technology Integration:**
- MediatR request/response pipeline
- FluentValidation in pipeline behaviors
- Entity Framework Core with DbContext
- Azure Service Bus for async webhook delivery
- Azure Redis Cache for rate limiting
- SignalR for real-time notifications
- Hangfire/Azure Functions for background jobs and retries
- HMAC-SHA256 for webhook signature verification

### 3. Component Diagram (`component-diagram.puml`)

**PlantUML component architecture** showing:
- Component relationships across all layers
- Dependency flow from UI to database
- MediatR pipeline execution
- Event dispatching mechanism
- Rate limiting middleware integration
- Webhook delivery service architecture
- Integration with external Azure services

## Technology Stack

### Backend
- **.NET 8** - Web API framework
- **MediatR** - CQRS and mediator pattern implementation
- **Entity Framework Core** - ORM and data access
- **FluentValidation** - Request validation
- **AutoMapper** - Object-to-object mapping
- **SignalR** - Real-time communication
- **Hangfire/Azure Functions** - Background job processing and webhook retries
- **ASP.NET Core Middleware** - Rate limiting and authentication

### Security
- **SHA-256** - API key hashing with salt
- **HMAC-SHA256** - Webhook payload signature verification
- **Azure Key Vault** - Encryption key management
- **JWT Bearer** - Authentication for admin endpoints

### Azure Services
- **Azure SQL Database** - Relational data storage
- **Azure Service Bus** - Message broker for webhook delivery and integration events
- **Azure Redis Cache** - Distributed rate limiting and caching
- **Azure Key Vault** - Secure key storage
- **Application Insights** - Monitoring and logging

## Implementation Guidelines

### Clean Architecture Principles

1. **Dependency Rule**: Dependencies point inward
   - Presentation → Application → Domain ← Infrastructure
   - Domain has no dependencies on other layers
   - Infrastructure implements interfaces defined in Domain/Application

2. **Separation of Concerns**
   - **Domain**: Business logic, aggregates, domain events, and invariants
   - **Application**: Use cases, command/query handlers, orchestration
   - **Infrastructure**: External concerns (database, Redis, Service Bus)
   - **Presentation**: API endpoints and middleware

3. **Testability**
   - Domain logic is pure and easily testable
   - Application handlers can be unit tested with mocked repositories
   - Integration tests for Infrastructure layer
   - End-to-end tests for complete workflows

### MediatR Pattern

```csharp
// Command
public record CreateApiKeyCommand(
    string KeyName,
    List<string> Permissions,
    Guid AccountId,
    Guid CreatedBy,
    DateTime? ExpiresAt) : IRequest<CreateApiKeyResponse>;

// Handler
public class CreateApiKeyHandler : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResponse>
{
    private readonly IApiKeyRepository _repository;
    private readonly ISecurityService _securityService;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<CreateApiKeyResponse> Handle(
        CreateApiKeyCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Generate cryptographically secure API key
        var plainTextKey = _securityService.GenerateApiKey();

        // 2. Hash the key (never store plain text)
        var hashedKey = _securityService.HashApiKey(plainTextKey);

        // 3. Create domain entity (raises domain events)
        var apiKey = ApiKey.Create(
            request.KeyName,
            hashedKey,
            request.AccountId,
            request.CreatedBy,
            request.Permissions,
            request.ExpiresAt);

        // 4. Persist to database
        await _repository.AddAsync(apiKey, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 5. Dispatch domain events (after save)
        await _eventDispatcher.DispatchAsync(apiKey.DomainEvents, cancellationToken);

        // 6. Return plain text key ONLY ONCE (never stored)
        return new CreateApiKeyResponse(
            apiKey.ApiKeyId,
            plainTextKey, // Only returned here, never again
            apiKey.CreatedAt,
            apiKey.ExpiresAt);
    }
}
```

### Domain-Driven Design

**Aggregate Roots:**

1. **ApiKey Aggregate**
   - Enforces API key security invariants
   - Raises domain events for lifecycle changes
   - Provides factory methods (`ApiKey.Create()`)
   - Manages permissions and expiration
   - Validates key status before operations

2. **Webhook Aggregate**
   - Manages webhook configuration and delivery
   - Enforces endpoint validation rules
   - Tracks delivery attempts and failures
   - Auto-disables after consecutive failures
   - Maintains delivery history

3. **ApiRequest Aggregate**
   - Records API usage for analytics
   - Tracks rate limiting enforcement
   - Provides audit trail for API access
   - Enables usage reporting and billing

**Domain Events:**
- Raised by aggregate roots for state changes
- Dispatched after successful persistence
- Enable event-driven architecture
- Trigger integration events and webhook delivery

**Value Objects:**
- `ApiKeyStatus`: Active, Revoked, Expired
- `WebhookStatus`: Active, Inactive, Failed
- `RateLimitTier`: Free (100/hour), Basic (1,000/hour), Pro (10,000/hour), Enterprise (100,000/hour), Unlimited
- `HttpMethod`: GET, POST, PUT, PATCH, DELETE

### Event-Driven Architecture

1. **Domain Events** (internal)
   - Raised within the domain boundary
   - Handled synchronously after SaveChanges
   - Examples: `ApiKeyCreatedDomainEvent`, `ApiKeyRevokedDomainEvent`, `WebhookRegisteredDomainEvent`

2. **Integration Events** (external)
   - Published to Azure Service Bus
   - Handled asynchronously by other services
   - Examples: Webhook delivery triggered, rate limit alerts, usage analytics

3. **Real-time Events** (SignalR)
   - Sent to connected clients
   - Enable real-time dashboard updates
   - Examples: API key created notification, webhook delivery status

## Key Workflows

### API Key Creation Flow
1. Developer initiates API key creation in Angular UI
2. HTTP POST to `/api/apikeys` with key name, permissions, and optional expiration
3. API Gateway authenticates with JWT Bearer token
4. `CreateApiKeyCommand` sent via MediatR pipeline
5. FluentValidation validates command (key name required, permissions valid, expiration in future)
6. Security service generates cryptographically secure 32-byte random key (Base64 encoded)
7. Security service hashes key with SHA-256 + salt
8. `ApiKey.Create()` creates aggregate and raises `ApiKeyCreatedDomainEvent`
9. Repository persists hashed key to database (plain text NEVER stored)
10. Domain events dispatched to Service Bus and SignalR
11. Response returns plain text key ONE TIME ONLY with warning message
12. Developer must save key immediately as it won't be shown again

### API Key Validation & Request Logging Flow
1. API client sends request with API key in header (`X-API-Key: sk_live_abc123...`)
2. API Gateway routes to Auth Middleware
3. Middleware extracts API key from header, query parameter, or Bearer token
4. `ValidateApiKeyQuery` sent via MediatR
5. Security service hashes provided key
6. Repository queries database for matching hashed key
7. Handler validates key status (Active, not expired, account active)
8. If invalid: Return 401 Unauthorized
9. If valid: Log request via `LogApiRequestCommand`
10. `ApiRequest.Create()` captures request metadata (endpoint, method, IP, user agent, timestamp)
11. Repository persists request log for analytics
12. `ApiRequestReceivedDomainEvent` published to Service Bus
13. HttpContext populated with ApiKeyId, AccountId, and Permissions
14. Request routed to resource controller
15. Controller checks permissions for requested operation
16. If insufficient permissions: Return 403 Forbidden
17. If authorized: Process request and return resource

### Rate Limiting Flow
1. Authenticated request enters Rate Limit Middleware
2. Middleware retrieves account context from previous auth middleware
3. Determines rate limit tier based on account subscription (Free, Basic, Pro, Enterprise, Unlimited)
4. Builds Redis cache key: `ratelimit:{accountId}:{hour}`
5. Queries Redis for current request count in this hour window
6. Compares current count with tier limit
7. **If limit exceeded:**
   - Increments Redis counter
   - `RecordRateLimitCommand` sent via MediatR
   - Request record marked as rate limited (`WasRateLimited = true`)
   - `ApiRequestRateLimitedDomainEvent` published to Service Bus (for alerts and upgrade prompts)
   - Calculates reset time (end of current hour)
   - Returns 429 Too Many Requests with headers:
     - `X-RateLimit-Limit`: Tier limit
     - `X-RateLimit-Remaining`: 0
     - `X-RateLimit-Reset`: Reset timestamp
     - `Retry-After`: Seconds until reset
8. **If within limit:**
   - Increments Redis counter with 1 hour TTL
   - Calculates remaining requests
   - Adds rate limit headers to response
   - Request continues to controller
   - Response returned with rate limit info headers

### Webhook Registration Flow
1. Developer initiates webhook registration in UI
2. HTTP POST to `/api/webhooks` with URL and event subscriptions
3. API Gateway authenticates request
4. `RegisterWebhookCommand` sent via MediatR
5. FluentValidation validates:
   - URL is required and valid HTTPS format
   - URL not localhost or private IP address
   - Domain is resolvable
   - Events list not empty
   - Event types are valid
6. Webhook Validation Service checks URL format
7. Service sends verification challenge: POST to `{url}/webhook-test` with random challenge string
8. **If endpoint unreachable or returns invalid response:**
   - Return 400 Bad Request: "Webhook endpoint verification failed"
9. **If endpoint responds with matching challenge:**
   - Check account webhook limit
   - If limit exceeded: Return 400 Bad Request
   - `Webhook.Register()` creates aggregate and raises `WebhookRegisteredDomainEvent`
   - Repository persists webhook configuration
   - Domain events dispatched for audit logging and monitoring setup
   - Return 201 Created with webhook details

### Webhook Delivery Flow
1. Domain event occurs (e.g., `ContentCreatedDomainEvent`)
2. Domain event handler queries active webhooks subscribed to event type
3. For each webhook: Publish `WebhookDeliveryMessage` to Service Bus queue
4. Background worker processes queue asynchronously
5. `TriggerWebhookCommand` sent via MediatR
6. Handler retrieves webhook and verifies active status
7. Builds webhook payload with event type, timestamp, and data
8. `WebhookDelivery.CreateAttempt()` creates delivery record with retry count
9. Repository persists delivery attempt
10. HTTP POST to webhook URL with:
    - Body: JSON payload
    - Headers: `X-Webhook-Signature` (HMAC-SHA256), `X-Event-Type`, `X-Webhook-Id`
    - Timeout: 30 seconds
11. **If delivery successful (2xx response):**
    - Measure response time
    - `webhookDelivery.MarkSuccess()` records status and time
    - `webhook.RecordSuccessfulDelivery()` resets failure count and updates last triggered time
    - Persist updates to database
    - Publish `WebhookTriggeredDomainEvent` for analytics
    - Notify via SignalR
12. **If delivery failed (timeout, network error, 5xx):**
    - `webhookDelivery.MarkFailure()` records reason
    - `webhook.RecordFailedDelivery()` increments failure count
    - If failure count >= 5: Auto-disable webhook
    - Publish `WebhookFailedDomainEvent` for monitoring and alerts
    - Check retry count (max 5 attempts)
    - If should retry: Schedule background job with exponential backoff:
      - Attempt 1: Immediate
      - Attempt 2: 5 minutes
      - Attempt 3: 30 minutes
      - Attempt 4: 2 hours
      - Attempt 5: 12 hours
    - If max retries exceeded: Webhook remains disabled, admin notification sent
13. **If delivery rejected (4xx client error):**
    - Mark failure without retry (except 429 Too Many Requests)
    - Log error for developer troubleshooting

## Database Schema

### ApiKeys Table
```sql
CREATE TABLE ApiKeys (
    ApiKeyId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KeyName NVARCHAR(200) NOT NULL,
    HashedKey NVARCHAR(512) NOT NULL,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    Permissions NVARCHAR(MAX), -- JSON array: ["content:read", "content:write"]
    ExpiresAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Status INT NOT NULL DEFAULT 0, -- 0=Active, 1=Revoked, 2=Expired
    RevokedAt DATETIME2 NULL,
    RevokedBy UNIQUEIDENTIFIER NULL,
    RevocationReason NVARCHAR(500) NULL,
    RowVersion ROWVERSION,

    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    FOREIGN KEY (RevokedBy) REFERENCES Users(UserId),

    CONSTRAINT UQ_ApiKeys_HashedKey UNIQUE (HashedKey)
);

CREATE INDEX IX_ApiKeys_AccountId ON ApiKeys(AccountId);
CREATE INDEX IX_ApiKeys_Status ON ApiKeys(Status) WHERE Status = 0; -- Active keys only
CREATE INDEX IX_ApiKeys_HashedKey ON ApiKeys(HashedKey); -- Fast key lookup
```

### Webhooks Table
```sql
CREATE TABLE Webhooks (
    WebhookId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Url NVARCHAR(2000) NOT NULL,
    Events NVARCHAR(MAX) NOT NULL, -- JSON array: ["content.created", "content.published"]
    AccountId UNIQUEIDENTIFIER NOT NULL,
    RegisteredBy UNIQUEIDENTIFIER NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- 0=Active, 1=Inactive, 2=Failed
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FailureCount INT NOT NULL DEFAULT 0,
    LastTriggeredAt DATETIME2 NULL,
    RowVersion ROWVERSION,

    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    FOREIGN KEY (RegisteredBy) REFERENCES Users(UserId),
    FOREIGN KEY (DeletedBy) REFERENCES Users(UserId)
);

CREATE INDEX IX_Webhooks_AccountId ON Webhooks(AccountId);
CREATE INDEX IX_Webhooks_Status ON Webhooks(Status) WHERE IsDeleted = 0;
CREATE INDEX IX_Webhooks_IsDeleted ON Webhooks(IsDeleted);
```

### WebhookDeliveries Table
```sql
CREATE TABLE WebhookDeliveries (
    DeliveryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    WebhookId UNIQUEIDENTIFIER NOT NULL,
    EventType NVARCHAR(100) NOT NULL,
    PayloadSent NVARCHAR(MAX) NOT NULL, -- JSON payload
    ResponseStatus INT NULL, -- HTTP status code
    FailureReason NVARCHAR(1000) NULL,
    RetryCount INT NOT NULL DEFAULT 0,
    IsSuccess BIT NOT NULL DEFAULT 0,
    AttemptedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    ResponseTime INT NULL, -- Milliseconds

    FOREIGN KEY (WebhookId) REFERENCES Webhooks(WebhookId) ON DELETE CASCADE
);

CREATE INDEX IX_WebhookDeliveries_WebhookId ON WebhookDeliveries(WebhookId);
CREATE INDEX IX_WebhookDeliveries_EventType ON WebhookDeliveries(EventType);
CREATE INDEX IX_WebhookDeliveries_AttemptedAt ON WebhookDeliveries(AttemptedAt);
CREATE INDEX IX_WebhookDeliveries_IsSuccess ON WebhookDeliveries(IsSuccess);
```

### ApiRequests Table
```sql
CREATE TABLE ApiRequests (
    RequestId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Endpoint NVARCHAR(500) NOT NULL,
    Method INT NOT NULL, -- 0=GET, 1=POST, 2=PUT, 3=PATCH, 4=DELETE
    ApiKeyId UNIQUEIDENTIFIER NOT NULL,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    IPAddress NVARCHAR(50) NOT NULL,
    ResponseStatus INT NULL, -- HTTP status code
    WasRateLimited BIT NOT NULL DEFAULT 0,
    RateLimitTier INT NULL, -- 0=Free, 1=Basic, 2=Pro, 3=Enterprise, 4=Unlimited
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ResponseTime INT NULL, -- Milliseconds
    RequestSize BIGINT NULL, -- Bytes
    ResponseSize BIGINT NULL, -- Bytes
    UserAgent NVARCHAR(500) NULL,

    FOREIGN KEY (ApiKeyId) REFERENCES ApiKeys(ApiKeyId),
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);

CREATE INDEX IX_ApiRequests_AccountId ON ApiRequests(AccountId);
CREATE INDEX IX_ApiRequests_ApiKeyId ON ApiRequests(ApiKeyId);
CREATE INDEX IX_ApiRequests_Timestamp ON ApiRequests(Timestamp);
CREATE INDEX IX_ApiRequests_WasRateLimited ON ApiRequests(WasRateLimited) WHERE WasRateLimited = 1;

-- Partitioning by month for performance (optional for high-volume scenarios)
-- ALTER TABLE ApiRequests ADD CONSTRAINT PK_ApiRequests_Timestamp
--     PRIMARY KEY (RequestId, Timestamp)
-- ON MonthlyPartitionScheme(Timestamp);
```

## Testing Strategy

### Unit Tests
- **Domain Entity Logic**
  - `ApiKey.Create()` validates parameters and raises events
  - `ApiKey.Revoke()` sets status and raises event
  - `Webhook.RecordFailedDelivery()` increments count and disables after 5 failures
  - Value object validation (RateLimitTier limits, status transitions)

- **Command/Query Handlers**
  - CreateApiKeyHandler with mocked repository and security service
  - ValidateApiKeyHandler with various key states (active, expired, revoked)
  - TriggerWebhookHandler with mocked HTTP client
  - Test exception handling and validation failures

- **Validators**
  - FluentValidation test helpers for all command validators
  - API key name validation (required, max length)
  - Webhook URL validation (HTTPS, valid format, no private IPs)
  - Permission validation (valid permission strings)

### Integration Tests
- **API Endpoints**
  - WebApplicationFactory for in-memory testing
  - Test complete request/response flow
  - JWT authentication and authorization
  - Rate limiting middleware integration

- **Repository Implementations**
  - Test database with migrations
  - CRUD operations for all aggregates
  - Query performance with realistic data volumes
  - Optimistic concurrency with RowVersion

- **Event Dispatching**
  - Domain events published after SaveChanges
  - Integration events sent to Service Bus (with test bus)
  - SignalR notifications to connected clients (with test hub)

- **Webhook Delivery**
  - Mock webhook endpoints with WireMock
  - Test successful delivery flow
  - Test retry logic with failures
  - Test exponential backoff timing

### E2E Tests
- **Complete User Workflows**
  - Create API key → Use key → View usage statistics
  - Register webhook → Trigger event → Verify delivery
  - Hit rate limit → Receive 429 → Wait for reset → Retry
  - Revoke API key → Verify subsequent requests fail

- **Performance Tests**
  - Load test rate limiting with concurrent requests
  - Webhook delivery throughput
  - API request logging performance impact

## Security Considerations

### API Key Security
- **Never Store Plain Text Keys**
  - Plain text key generated with cryptographically secure random number generator
  - Key immediately hashed with SHA-256 + salt before storage
  - Plain text key returned only once during creation
  - Database contains only hashed values

- **Key Hashing**
  ```csharp
  // Secure key generation
  var keyBytes = new byte[32];
  using var rng = RandomNumberGenerator.Create();
  rng.GetBytes(keyBytes);
  var plainTextKey = $"sk_live_{Convert.ToBase64String(keyBytes)}";

  // Hashing with salt
  var hashedKey = _securityService.HashApiKey(plainTextKey);
  // Uses SHA-256 with account-specific salt from Key Vault
  ```

- **Key Validation**
  - Hash provided key and compare with stored hash
  - Constant-time comparison to prevent timing attacks
  - Check expiration date
  - Verify account active status
  - Validate key has required permissions

### Webhook Security
- **Endpoint Validation**
  - Must be HTTPS (no HTTP)
  - No localhost or private IP addresses (127.0.0.1, 192.168.x.x, 10.x.x.x)
  - Domain must be publicly resolvable
  - Verification challenge sent during registration

- **HMAC Signature Verification**
  ```csharp
  // Webhook payload signing
  var secret = _securityService.GetWebhookSecret(accountId);
  var signature = ComputeHmacSha256(payload, secret);

  // Include in request headers
  headers.Add("X-Webhook-Signature", signature);
  headers.Add("X-Event-Type", eventType);
  headers.Add("X-Webhook-Id", webhookId.ToString());
  ```

- **Webhook Recipient Verification**
  ```csharp
  // Recipient verifies signature
  var receivedSignature = request.Headers["X-Webhook-Signature"];
  var computedSignature = ComputeHmacSha256(requestBody, secret);

  if (!TimingSafeEquals(receivedSignature, computedSignature))
  {
      return Unauthorized("Invalid signature");
  }
  ```

### Rate Limiting Security
- **DDoS Protection**
  - Rate limits prevent API abuse
  - Per-account limits based on subscription tier
  - IP-based rate limiting for unauthenticated endpoints
  - Exponential backoff recommended in 429 responses

- **Redis Security**
  - SSL/TLS encryption for Redis connections
  - Authentication required (Redis AUTH)
  - Network isolation with VNet integration
  - Firewall rules restrict access

### Authentication & Authorization
- **Admin Endpoints**
  - JWT Bearer token authentication
  - Claims-based authorization (AccountId, UserId, Roles)
  - Validate user has permission to manage API keys for account
  - Audit logging for all administrative actions

- **API Endpoints**
  - API key authentication via header, query, or Bearer token
  - Permission-based authorization (e.g., "content:read", "content:write")
  - Validate API key has required permission for operation
  - Request logging for audit trail

### Data Protection
- **Encryption at Rest**
  - Azure SQL Database Transparent Data Encryption (TDE)
  - Backup encryption enabled

- **Encryption in Transit**
  - HTTPS/TLS 1.2+ for all API communications
  - SSL for Redis connections
  - Service Bus encrypted transport

- **Sensitive Data**
  - API keys never logged
  - Webhook payloads sanitized in logs (PII removed)
  - Secrets stored in Azure Key Vault

## Performance Optimizations

### Redis Caching Strategy
- **Rate Limit Counters**
  - Sliding window algorithm using sorted sets
  - Key pattern: `ratelimit:{accountId}:{hour}`
  - TTL set to 1 hour for automatic cleanup
  - INCR operation is atomic and fast (O(1))

- **API Key Validation Cache**
  - Cache valid API keys for 5 minutes
  - Invalidate on key revocation
  - Reduces database load for high-frequency requests
  - Key pattern: `apikey:{hashedKey}`

- **Webhook Configuration Cache**
  - Cache active webhooks per account
  - Invalidate on webhook updates
  - Speeds up event-to-webhook matching
  - Key pattern: `webhooks:{accountId}`

### Database Optimization
- **Indexed Queries**
  - Composite indexes on frequently queried columns
  - Covering indexes for common queries
  - Filtered indexes for active records only

- **Pagination**
  - Skip/Take for large result sets
  - Cursor-based pagination for API usage logs
  - Limit max page size (e.g., 100 records)

- **Query Optimization**
  - AsNoTracking for read-only queries
  - Compiled queries for frequently executed queries
  - Batch operations for bulk updates

- **Partitioning** (for high-volume scenarios)
  - Partition ApiRequests table by month
  - Archive old data to cold storage
  - Improves query performance and index maintenance

### Asynchronous Operations
- **Webhook Delivery**
  - Fire-and-forget via Service Bus queue
  - Decouples request processing from delivery
  - Enables retry without blocking
  - Parallel delivery for multiple webhooks

- **Event Publishing**
  - Domain events dispatched asynchronously after SaveChanges
  - Integration events published to Service Bus
  - Non-blocking real-time notifications via SignalR

- **Background Jobs**
  - Hangfire/Azure Functions for scheduled tasks
  - Retry failed webhook deliveries
  - Cleanup expired API keys
  - Aggregate usage statistics

### Connection Pooling
- **Database Connections**
  - EF Core connection pooling enabled
  - Reuse DbContext instances where appropriate
  - Monitor connection pool exhaustion

- **HTTP Client**
  - HttpClientFactory for webhook delivery
  - Connection pooling and DNS refresh
  - Timeout configuration (30 seconds)

## Monitoring & Logging

### Application Insights
- **Request Telemetry**
  - Track all API requests with custom dimensions
  - API key ID (hashed)
  - Account ID
  - Rate limit tier
  - Response status and time

- **Dependency Telemetry**
  - Database query performance
  - Redis cache operations
  - Service Bus message publishing
  - Webhook HTTP calls

- **Exception Tracking**
  - Unhandled exceptions with stack traces
  - Validation failures
  - Webhook delivery failures
  - Rate limit exceeded events

- **Custom Events**
  - API key created/revoked
  - Webhook registered/disabled
  - Rate limit exceeded
  - Webhook delivery success/failure

### Structured Logging
- **Serilog Configuration**
  ```csharp
  Log.Logger = new LoggerConfiguration()
      .MinimumLevel.Information()
      .Enrich.FromLogContext()
      .Enrich.WithProperty("Application", "API Management")
      .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
      .WriteTo.Console(new JsonFormatter())
      .CreateLogger();
  ```

- **Correlation IDs**
  - Track requests across services
  - Include in all log entries
  - Propagate via Service Bus messages
  - Enable end-to-end tracing

- **Log Levels**
  - **Debug**: Detailed flow information (disabled in production)
  - **Information**: API key operations, webhook deliveries
  - **Warning**: Rate limit approaching, webhook retries
  - **Error**: Validation failures, delivery failures
  - **Critical**: Service Bus outages, database failures

### Metrics & Dashboards
- **Key Performance Indicators (KPIs)**
  - API requests per minute/hour/day
  - Rate limit hit rate (429 responses)
  - Webhook delivery success rate
  - Average webhook delivery time
  - API key creation rate

- **Availability Metrics**
  - API endpoint uptime (99.9% SLA)
  - Database connection health
  - Redis cache availability
  - Service Bus queue depth

- **Performance Metrics**
  - API response time (p50, p95, p99)
  - Database query duration
  - Redis operation latency
  - Webhook delivery latency

### Alerts & Notifications
- **Critical Alerts**
  - API endpoint down (5xx error rate > 5%)
  - Database connection failures
  - Redis cache unavailable
  - Service Bus queue depth > 10,000 messages

- **Warning Alerts**
  - Webhook failure rate > 20%
  - Rate limit exceeded for account (upgrade prompt)
  - API key expiring soon (7 days)
  - High request latency (p95 > 1 second)

- **Dashboard Visualizations**
  - Real-time request rate graph
  - Rate limit usage by tier
  - Webhook delivery status
  - Top API consumers by account
  - Error rate trends

## Next Steps

1. **Implementation**: Follow the model definitions in `/docs/model/api.md`
2. **Database**: Create EF Core migrations for ApiKeys, Webhooks, WebhookDeliveries, and ApiRequests tables
3. **Security**: Configure Azure Key Vault for encryption keys and secrets
4. **Infrastructure**: Set up Azure Redis Cache and Service Bus
5. **Testing**: Implement unit, integration, and E2E tests
6. **Deployment**: Configure Azure resources and CI/CD pipeline
7. **Documentation**: Generate API documentation with Swagger/OpenAPI
8. **Monitoring**: Configure Application Insights dashboards and alerts

## References

- Event Definitions: `/docs/events/api.md`
- Model Definitions: `/docs/model/api.md`
- Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- MediatR Documentation: https://github.com/jbogard/MediatR
- Domain-Driven Design: Eric Evans, "Domain-Driven Design"
- Rate Limiting Patterns: https://docs.microsoft.com/en-us/azure/architecture/patterns/rate-limiting-pattern
- Webhook Security: https://webhooks.fyi/security/hmac

---

**Last Updated**: December 2024
**Version**: 1.0
**Status**: Design Complete - Ready for Implementation
