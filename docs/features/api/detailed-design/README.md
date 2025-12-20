# API Management - Detailed Design

## Overview

API Management handles API keys, webhooks, and API request tracking for the StartupStarter system.

## Key Components

### Aggregates

**ApiKeyAggregate**
- Manages API key lifecycle
- Tracks permissions and expiration
- Handles revocation

**WebhookAggregate**
- Webhook registration and configuration
- Delivery tracking
- Retry logic for failed deliveries

## Dependencies

- **AccountAggregate**: API keys belong to accounts
- **AuditAggregate**: All API operations are audited
- **External HTTP Client**: For webhook delivery

## Constraints

### Business Rules

1. **API Key Management**
   - API keys must be unique
   - Keys are hashed before storage (never store plaintext)
   - Expired keys automatically deactivated
   - Revoked keys cannot be reactivated

2. **Webhook Management**
   - Webhook URLs must be HTTPS
   - Maximum 5 retry attempts for failed deliveries
   - Exponential backoff between retries
   - Webhooks auto-disabled after 10 consecutive failures

3. **Rate Limiting**
   - Per-account rate limits based on subscription tier
   - Per-endpoint rate limits
   - Burst allowance for short spikes

### Technical Constraints

- API key validation must be < 50ms
- Webhook delivery timeout: 30 seconds
- Maximum webhook payload size: 1MB
- Request logging asynchronous to not impact performance

## Sequence Diagrams

### Create API Key
```
User → API → CreateApiKeyHandler
→ Generate secure random key
→ Hash key for storage
→ Create ApiKey aggregate
→ Save to database
→ Return key to user (only time shown in plaintext)
```

### Webhook Delivery
```
Event occurs → Event Handler
→ Find webhooks subscribed to event
→ For each webhook:
  → Send HTTP POST to webhook URL
  → Record delivery attempt
  → If failed: Schedule retry with backoff
  → If succeeded: Update delivery record
```

## Data Model

### ApiKeys Table
- ApiKeyId (PK)
- KeyHash (unique, indexed)
- AccountId (FK, indexed)
- KeyName
- Permissions (JSON)
- ExpiresAt
- RevokedAt

### Webhooks Table
- WebhookId (PK)
- AccountId (FK, indexed)
- Url
- Events (JSON array)
- IsActive
- FailureCount

### WebhookDeliveries Table
- WebhookDeliveryId (PK)
- WebhookId (FK)
- EventType
- PayloadJson
- ResponseStatus
- Success
- RetryCount
- Timestamp (indexed)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/keys | Create API key |
| GET | /api/keys | List account API keys |
| DELETE | /api/keys/{id} | Revoke API key |
| POST | /api/webhooks | Register webhook |
| GET | /api/webhooks | List account webhooks |
| PUT | /api/webhooks/{id} | Update webhook |
| DELETE | /api/webhooks/{id} | Delete webhook |
| GET | /api/webhooks/{id}/deliveries | Get delivery history |

## Security

- API keys transmitted only over HTTPS
- Keys hashed using bcrypt/Argon2
- Webhook signatures using HMAC-SHA256
- IP whitelisting support for API keys
