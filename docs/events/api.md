# API Events

## API.Key.Created
**Description**: Fired when a new API key is generated

**Payload**:
- ApiKeyId: string
- KeyName: string
- AccountId: string
- CreatedBy: string (UserId)
- Permissions: List<string>
- ExpiresAt: DateTime (optional)
- Timestamp: DateTime

---

## API.Key.Revoked
**Description**: Fired when an API key is revoked

**Payload**:
- ApiKeyId: string
- AccountId: string
- RevokedBy: string (UserId or AdminId)
- Reason: string
- Timestamp: DateTime

---

## API.Request.Received
**Description**: Fired when an API request is received

**Payload**:
- RequestId: string
- Endpoint: string
- Method: enum (GET, POST, PUT, PATCH, DELETE)
- ApiKeyId: string
- AccountId: string
- IPAddress: string
- Timestamp: DateTime

---

## API.Request.RateLimited
**Description**: Fired when an API request is rate limited

**Payload**:
- RequestId: string
- Endpoint: string
- ApiKeyId: string
- AccountId: string
- RateLimitTier: string
- Timestamp: DateTime

---

## API.Webhook.Registered
**Description**: Fired when a webhook is registered

**Payload**:
- WebhookId: string
- Url: string
- Events: List<string>
- AccountId: string
- RegisteredBy: string (UserId)
- Timestamp: DateTime

---

## API.Webhook.Triggered
**Description**: Fired when a webhook is triggered

**Payload**:
- WebhookId: string
- AccountId: string
- EventType: string
- PayloadSent: object
- ResponseStatus: int
- Timestamp: DateTime

---

## API.Webhook.Failed
**Description**: Fired when a webhook delivery fails

**Payload**:
- WebhookId: string
- AccountId: string
- EventType: string
- FailureReason: string
- RetryCount: int
- Timestamp: DateTime

---

## API.Webhook.Deleted
**Description**: Fired when a webhook is deleted

**Payload**:
- WebhookId: string
- AccountId: string
- DeletedBy: string (UserId)
- Timestamp: DateTime
