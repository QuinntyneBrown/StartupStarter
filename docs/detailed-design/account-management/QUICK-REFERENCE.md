# Account Management - Quick Reference Guide

## 🎯 For Business Stakeholders

### What is Account Management?
The system that handles organization accounts (tenants) in the CMS platform.

### Key Capabilities
- ✅ Create and configure accounts
- ✅ Manage subscription tiers
- ✅ Suspend/reactivate accounts
- ✅ Transfer account ownership
- ✅ Configure account settings
- ✅ Associate profiles with accounts

### Account Lifecycle
```
[Created] → [Active] → [Suspended] → [Deleted]
                ↓           ↓
            [Active]    [Deleted]
```

### Business Events
Every action produces an event for auditing and integration:
- Account created/updated/deleted
- Subscription changed
- Account suspended/reactivated
- Owner changed
- Settings updated
- Profile added/removed

---

## 👨‍💻 For Developers

### Architecture Patterns
- **DDD:** Domain-Driven Design
- **Event Sourcing:** State from events
- **CQRS:** Separate read/write models
- **Clean Architecture:** Layered design
- **MediatR v12.x:** Command/query dispatch

### MediatR v12.4.1 Setup
```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

```csharp
// Program.cs
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
```

### Project Structure
```
AccountManagement/
├── Domain/
│   ├── Account.cs (Aggregate Root)
│   ├── Events/
│   └── ValueObjects/
├── Application/
│   ├── Commands/
│   │   ├── CreateAccount/
│   │   │   ├── CreateAccountCommand.cs
│   │   │   └── CreateAccountHandler.cs (IRequestHandler)
│   ├── Queries/
│   │   ├── GetAccountById/
│   │   │   ├── GetAccountByIdQuery.cs
│   │   │   └── GetAccountByIdHandler.cs (IRequestHandler)
│   └── Notifications/
│       └── AccountCreatedHandler.cs (INotificationHandler)
├── Infrastructure/
│   ├── EventStore/
│   ├── ReadModels/
│   └── Messaging/
└── API/
    └── Controllers/
```

### Key Commands (MediatR Requests)
```csharp
public record CreateAccountCommand(
    string AccountName,
    AccountType AccountType,
    string OwnerUserId,
    string SubscriptionTier
) : IRequest<CreateAccountResult>;

public record UpdateAccountCommand(
    string AccountId,
    Dictionary<string, object> Fields
) : IRequest<UpdateAccountResult>;
```

### Key Queries (MediatR Requests)
```csharp
public record GetAccountByIdQuery(
    string AccountId
) : IRequest<AccountDto>;

public record SearchAccountsQuery(
    string? SearchTerm,
    int Page,
    int PageSize
) : IRequest<PagedResult<AccountDto>>;
```

### Key Handlers
```csharp
// Command Handler
public class CreateAccountHandler 
    : IRequestHandler<CreateAccountCommand, CreateAccountResult>
{
    public async Task<CreateAccountResult> Handle(
        CreateAccountCommand request, 
        CancellationToken cancellationToken)
    {
        // Load or create aggregate
        // Execute business logic
        // Save events
        // Publish domain events
        return result;
    }
}

// Query Handler
public class GetAccountByIdHandler 
    : IRequestHandler<GetAccountByIdQuery, AccountDto>
{
    public async Task<AccountDto> Handle(
        GetAccountByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // Query read model
        return accountDto;
    }
}

// Notification Handler (Domain Events)
public class AccountCreatedHandler 
    : INotificationHandler<AccountCreatedNotification>
{
    public async Task Handle(
        AccountCreatedNotification notification, 
        CancellationToken cancellationToken)
    {
        // Update projections
        // Send integrations events
    }
}
```

### Technology Stack
- .NET 8.0 / C#
- ASP.NET Core Web API
- MediatR v12.4.1 (Free)
- EventStoreDB / Cosmos DB
- Azure Service Bus
- Redis Cache
- Azure Kubernetes Service

---

## 🧪 For QA Engineers

### Test Pyramid
```
        E2E Tests (~50)
    ─────────────────────
    Integration Tests (~200)
  ───────────────────────────
  Unit Tests (~500+)
```

### Testing MediatR Handlers

**Unit Test Example:**
```csharp
[Fact]
public async Task CreateAccountHandler_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var command = new CreateAccountCommand(
        "Acme Corp", 
        AccountType.Enterprise,
        "user-123",
        "Premium"
    );
    var handler = new CreateAccountHandler(
        mockEventStore.Object,
        mockPublisher.Object
    );

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
}
```

### Key Test Scenarios

**Happy Path:**
1. Create account → Success
2. Update account → Success
3. Query account → Returns data
4. Suspend account → Access denied
5. Reactivate → Access restored

**Edge Cases:**
1. Create duplicate account → Error
2. Suspend suspended account → Error
3. Delete non-existent account → Error
4. Update with invalid data → Validation error
5. Concurrent updates → Optimistic concurrency handled

**Integration Tests:**
1. Event publishing → Subscribers notified
2. Projection updates → Read model consistent
3. Cross-boundary → Other BCs react correctly

### Test Data Builders
```csharp
var account = new AccountBuilder()
    .WithName("Acme Corp")
    .WithType(AccountType.Enterprise)
    .WithOwner("user-123")
    .WithSubscription("Premium")
    .Build();
```

---

## 📊 For Product Managers

### Metrics to Track
- Accounts created per day/week/month
- Subscription tier distribution
- Account suspension rate
- Account deletion reasons
- Time to activation
- Feature usage by tier

### Feature Flags
- `EnableAccountSuspension`
- `EnableOwnershipTransfer`
- `EnableMultipleProfiles`
- `EnableAdvancedSettings`

### Business Rules
1. Each user has one account
2. Account can have many profiles
3. Deleted accounts cannot be reactivated
4. Suspended accounts have limited access
5. Subscription changes affect features immediately
6. Owner transfer requires authorization

---

## 🔐 For Security Team

### Security Measures
- ✅ Authentication: Azure AD B2C
- ✅ Authorization: Role-based + Policy-based
- ✅ Encryption: At rest and in transit
- ✅ Secrets: Azure Key Vault
- ✅ Audit: All actions logged
- ✅ Rate Limiting: API throttling
- ✅ Input Validation: All inputs validated

### Compliance
- ✅ GDPR: Data deletion support
- ✅ SOC 2: Audit trail
- ✅ PCI DSS: If handling payments
- ✅ Data Residency: Multi-region support

---

## 📈 For Operations Team

### Monitoring
- **Health Checks:** `/health/live`, `/health/ready`
- **Metrics:** Prometheus format at `/metrics`
- **Logs:** Structured logs to Application Insights
- **Traces:** Distributed tracing enabled

### Key Metrics
- Command processing time (target: <100ms p95)
- Query response time (target: <50ms p95)
- Event processing lag (target: <1 second)
- Error rate (target: <0.1%)
- MediatR handler execution time

### Scaling
- **Command API:** 3-10 pods (CPU/memory based)
- **Query API:** 5-15 pods (request rate based)
- **Projections:** 3 pods (competing consumers)

### Deployment
- **Strategy:** Blue-green deployment
- **Rollback:** Automated on health check failure
- **Zero Downtime:** Yes
- **Rollout Duration:** ~10 minutes

---

## 📞 Quick Links

### Documentation
- 📖 [Full README](README.md)
- 🎨 [PlantUML Diagrams](*.puml)
- 📋 [API Documentation](swagger-url)
- 📝 [ADRs](adrs/)
- 🔧 [MediatR Docs](https://github.com/jbogard/MediatR)

### Tools
- 🔍 [Application Insights](azure-portal-link)
- 📊 [Grafana Dashboards](grafana-link)
- 🐛 [Issue Tracker](jira-link)
- 💬 [Team Chat](slack-channel)

### Contacts
- 🏗️ Architecture: architecture@contently.com
- 👨‍💻 Development: dev@contently.com
- 🧪 QA: qa@contently.com
- 🚀 DevOps: devops@contently.com

---

## 🚀 Getting Started

### For Developers
```bash
# Clone repository
git clone [repo-url]

# Restore packages (includes MediatR v12.4.1)
dotnet restore

# Run locally
dotnet run --project src/AccountManagement.API

# Run tests
dotnet test

# View API docs
http://localhost:5000/swagger
```

### For QA
```bash
# Run all tests
dotnet test

# Run specific category
dotnet test --filter Category=Integration

# Generate coverage report
dotnet test /p:CollectCoverage=true
```

---

**Last Updated:** December 2024  
**Version:** 1.1  
**MediatR Version:** 12.4.1 (Free)
