# Account Management - Implementation Checklist

## Phase 1: Foundation (Week 1-2)

### Setup & Dependencies
- [ ] Create solution structure following Clean Architecture
- [ ] Install NuGet packages
  - [ ] `MediatR` v12.4.1
  - [ ] `MediatR.Extensions.Microsoft.DependencyInjection` v12.0.0
  - [ ] `FluentValidation` v11.9.0
  - [ ] `FluentValidation.DependencyInjectionExtensions` v11.9.0

### Domain Layer
- [ ] Create `Account` aggregate root
  - [ ] AccountId value object
  - [ ] UserId value object
  - [ ] ProfileId value object
  - [ ] AccountSettings value object
- [ ] Implement enums
  - [ ] AccountType
  - [ ] AccountStatus
  - [ ] DeletionType
- [ ] Define all domain events
  - [ ] AccountCreated
  - [ ] AccountUpdated
  - [ ] AccountDeleted
  - [ ] AccountSuspended
  - [ ] AccountReactivated
  - [ ] AccountSubscriptionChanged
  - [ ] AccountOwnerChanged
  - [ ] AccountSettingsUpdated
  - [ ] AccountProfileAdded
  - [ ] AccountProfileRemoved
- [ ] Implement business rules
  - [ ] Account creation validation
  - [ ] Status transition rules
  - [ ] Subscription change validation
  - [ ] Owner transfer validation
- [ ] Write unit tests for domain logic (target: 95% coverage)

---

## Phase 2: Application Layer with MediatR (Week 2-3)

### MediatR Configuration
- [ ] Register MediatR in `Program.cs`
```csharp
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
```
- [ ] Configure FluentValidation pipeline behavior
- [ ] Add logging pipeline behavior
- [ ] Add transaction pipeline behavior (optional)

### Commands (IRequest<TResponse>)
- [ ] Define command records
  - [ ] `CreateAccountCommand : IRequest<CreateAccountResult>`
  - [ ] `UpdateAccountCommand : IRequest<UpdateAccountResult>`
  - [ ] `DeleteAccountCommand : IRequest<DeleteAccountResult>`
  - [ ] `SuspendAccountCommand : IRequest<SuspendAccountResult>`
  - [ ] `ReactivateAccountCommand : IRequest<ReactivateAccountResult>`
  - [ ] `ChangeSubscriptionCommand : IRequest<ChangeSubscriptionResult>`
  - [ ] `ChangeOwnerCommand : IRequest<ChangeOwnerResult>`
  - [ ] `UpdateSettingsCommand : IRequest<UpdateSettingsResult>`
  - [ ] `AddProfileCommand : IRequest<AddProfileResult>`
  - [ ] `RemoveProfileCommand : IRequest<RemoveProfileResult>`

### Command Handlers (IRequestHandler<TRequest, TResponse>)
- [ ] Implement command handlers using MediatR
  - [ ] `CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResult>`
  - [ ] `UpdateAccountHandler`
  - [ ] `DeleteAccountHandler`
  - [ ] `SuspendAccountHandler`
  - [ ] `ReactivateAccountHandler`
  - [ ] `ChangeSubscriptionHandler`
  - [ ] `ChangeOwnerHandler`
  - [ ] `UpdateSettingsHandler`
  - [ ] `AddProfileHandler`
  - [ ] `RemoveProfileHandler`
- [ ] Add FluentValidation validators for each command
- [ ] Implement authorization checks
- [ ] Write unit tests for handlers

### Queries (IRequest<TResponse>)
- [ ] Define query records
  - [ ] `GetAccountByIdQuery : IRequest<AccountDto>`
  - [ ] `GetAccountsByOwnerQuery : IRequest<List<AccountDto>>`
  - [ ] `GetAccountsByTypeQuery : IRequest<List<AccountDto>>`
  - [ ] `GetAccountsByStatusQuery : IRequest<List<AccountDto>>`
  - [ ] `SearchAccountsQuery : IRequest<PagedResult<AccountDto>>`

### Query Handlers (IRequestHandler<TRequest, TResponse>)
- [ ] Implement query handlers
  - [ ] `GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>`
  - [ ] `GetAccountsByOwnerHandler`
  - [ ] `GetAccountsByTypeHandler`
  - [ ] `GetAccountsByStatusHandler`
  - [ ] `SearchAccountsHandler`
- [ ] Define read model DTOs
  - [ ] AccountDto
  - [ ] AccountListDto
  - [ ] AccountSummaryDto
- [ ] Write unit tests for queries

### Domain Event Notifications (INotification)
- [ ] Define notification classes implementing `INotification`
  - [ ] `AccountCreatedNotification : INotification`
  - [ ] `AccountUpdatedNotification : INotification`
  - [ ] etc...
- [ ] Implement notification handlers using `INotificationHandler<TNotification>`
  - [ ] Update projections
  - [ ] Publish integration events
  - [ ] Trigger side effects

### Interfaces
- [ ] Define `IEventStore` interface
- [ ] Define `IEventPublisher` interface
- [ ] Define `IAccountReadModelRepository` interface

---

## Phase 3: Infrastructure Layer (Week 3-4)

### Event Store
- [ ] Choose event store implementation (EventStoreDB / Cosmos DB)
- [ ] Implement `IEventStore`
  - [ ] AppendToStream method
  - [ ] ReadStream method
  - [ ] Optimistic concurrency control
- [ ] Implement snapshot support (optional)
- [ ] Add event serialization
- [ ] Configure event store connection

### Event Publishing
- [ ] Implement `IEventPublisher`
- [ ] Choose message bus (Azure Service Bus / RabbitMQ / Kafka)
- [ ] Configure topic/queue structure
- [ ] Implement at-least-once delivery
- [ ] Add retry logic
- [ ] Implement dead letter queue

### Read Models
- [ ] Choose read model database (Cosmos DB / SQL Server)
- [ ] Implement `IAccountReadModelRepository`
- [ ] Create database schema/containers
- [ ] Add indexes for common queries
- [ ] Implement caching layer (Redis)

### Projections
- [ ] Implement AccountProjection (INotificationHandler)
  - [ ] Handle AccountCreatedNotification
  - [ ] Handle AccountUpdatedNotification
  - [ ] Handle AccountDeletedNotification
  - [ ] Handle AccountSuspendedNotification
  - [ ] Handle AccountReactivatedNotification
  - [ ] Handle AccountSubscriptionChangedNotification
  - [ ] Handle AccountOwnerChangedNotification
  - [ ] Handle AccountSettingsUpdatedNotification
  - [ ] Handle AccountProfileAddedNotification
  - [ ] Handle AccountProfileRemovedNotification
- [ ] Add idempotency handling
- [ ] Implement projection rebuild capability
- [ ] Add error handling and retry logic

### Integration Tests
- [ ] Write event store integration tests
- [ ] Write event publisher integration tests
- [ ] Write projection tests
- [ ] Use Testcontainers for dependencies
- [ ] Test optimistic concurrency scenarios
- [ ] Test MediatR pipeline integration

---

## Phase 4: API Layer (Week 4-5)

### Controllers
- [ ] Implement AccountCommandController
  - Inject `ISender` from MediatR
  - [ ] POST /api/accounts (create)
  - [ ] PUT /api/accounts/{id} (update)
  - [ ] DELETE /api/accounts/{id} (delete)
  - [ ] POST /api/accounts/{id}/suspend
  - [ ] POST /api/accounts/{id}/reactivate
  - [ ] PUT /api/accounts/{id}/subscription
  - [ ] PUT /api/accounts/{id}/owner
  - [ ] PUT /api/accounts/{id}/settings
  - [ ] POST /api/accounts/{id}/profiles
  - [ ] DELETE /api/accounts/{id}/profiles/{profileId}
- [ ] Implement AccountQueryController
  - Inject `ISender` from MediatR
  - [ ] GET /api/accounts/{id}
  - [ ] GET /api/accounts (with filters)
  - [ ] GET /api/accounts/search

### Example Controller Implementation
```csharp
[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ISender _sender;
    
    public AccountsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(
            request.AccountName,
            request.AccountType,
            request.OwnerUserId,
            request.SubscriptionTier
        );
        
        var result = await _sender.Send(command, cancellationToken);
        
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetAccount), new { id = result.AccountId }, result)
            : BadRequest(result.Error);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetAccountByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        
        return result != null 
            ? Ok(result)
            : NotFound();
    }
}
```

### API Features
- [ ] Add Swagger/OpenAPI documentation
- [ ] Implement API versioning
- [ ] Add request/response logging
- [ ] Implement correlation IDs
- [ ] Add rate limiting
- [ ] Configure CORS
- [ ] Implement health checks
  - [ ] Liveness probe
  - [ ] Readiness probe
  - [ ] Dependency health checks

### Security
- [ ] Integrate Azure AD B2C / OAuth
- [ ] Implement JWT authentication
- [ ] Add role-based authorization
- [ ] Implement policy-based authorization
- [ ] Add input validation
- [ ] Implement HTTPS enforcement
- [ ] Add security headers

---

## Phase 5: Testing (Week 5-6)

### Unit Tests
- [ ] Domain layer tests (500+ tests)
- [ ] Application layer tests
- [ ] Test MediatR handlers in isolation
  ```csharp
  // Mock ISender and IPublisher
  var mockSender = new Mock<ISender>();
  var mockPublisher = new Mock<IPublisher>();
  ```
- [ ] Achieve 95%+ coverage for domain
- [ ] Achieve 85%+ coverage for application

### Integration Tests
- [ ] Command handler integration tests (200+ tests)
- [ ] Test full MediatR pipeline
- [ ] Projection integration tests
- [ ] Event publishing tests
- [ ] Read model tests

### End-to-End Tests
- [ ] API endpoint tests using Playwright/REST Client (50+ tests)
- [ ] Complete workflow tests
- [ ] Performance tests
- [ ] Load tests

### Contract Tests
- [ ] API contract tests using Pact (30+ contracts)
- [ ] Event schema validation
- [ ] Backward compatibility tests

---

## Phase 6: DevOps & Deployment (Week 6-7)

### Containerization
- [ ] Create Dockerfile for API
- [ ] Create Dockerfile for projection workers
- [ ] Optimize image size
- [ ] Add health check endpoint
- [ ] Test containers locally

### Kubernetes
- [ ] Create deployment manifests
  - [ ] Command API deployment
  - [ ] Query API deployment
  - [ ] Projection worker deployment
- [ ] Create service manifests
- [ ] Configure resource limits
- [ ] Set up horizontal pod autoscaling
- [ ] Configure liveness/readiness probes
- [ ] Create ConfigMaps
- [ ] Create Secrets

### CI/CD Pipeline
- [ ] Set up build pipeline
  - [ ] Restore dependencies (including MediatR)
  - [ ] Build solution
  - [ ] Run unit tests
  - [ ] Run static code analysis
  - [ ] Build Docker images
- [ ] Set up release pipeline
  - [ ] Deploy to Dev (automatic)
  - [ ] Run integration tests
  - [ ] Deploy to Staging (automatic)
  - [ ] Run E2E tests
  - [ ] Deploy to Production (manual approval)
  - [ ] Run smoke tests
- [ ] Configure blue-green deployment
- [ ] Implement automated rollback

### Infrastructure as Code
- [ ] Create Terraform/Bicep templates
  - [ ] AKS cluster
  - [ ] Cosmos DB
  - [ ] Service Bus
  - [ ] Redis Cache
  - [ ] Application Insights
  - [ ] Key Vault
- [ ] Set up environments (Dev, Staging, Prod)
- [ ] Configure networking and security

---

## Phase 7: Monitoring & Operations (Week 7-8)

### Observability
- [ ] Configure Application Insights
- [ ] Set up structured logging (Serilog)
- [ ] Implement distributed tracing
- [ ] Add custom metrics
  - [ ] Command processing time
  - [ ] Query response time
  - [ ] Event processing lag
  - [ ] Error rates
  - [ ] MediatR handler execution time
- [ ] Create Grafana dashboards
- [ ] Set up alerts
  - [ ] High error rate
  - [ ] Slow response times
  - [ ] Event processing lag
  - [ ] Resource saturation

### Documentation
- [ ] Write API documentation
- [ ] Create runbooks
  - [ ] Deployment procedures
  - [ ] Rollback procedures
  - [ ] Troubleshooting guide
  - [ ] Disaster recovery
- [ ] Document architecture decisions (ADRs)
- [ ] Create developer onboarding guide
- [ ] Write operational procedures
- [ ] Document MediatR patterns used

---

## Phase 8: Integration & Polish (Week 8)

### Bounded Context Integration
- [ ] Integrate with User Management BC
  - [ ] Subscribe to user events
  - [ ] Validate owner on account creation
- [ ] Integrate with Profile Management BC
  - [ ] Publish profile addition/removal events
  - [ ] Handle profile lifecycle
- [ ] Integrate with Billing BC
  - [ ] Publish subscription change events
  - [ ] Handle billing notifications
- [ ] Integrate with Audit Log BC
  - [ ] Publish all events to audit
- [ ] Integrate with Notification Service
  - [ ] Send account creation emails
  - [ ] Send suspension notifications

### Performance Optimization
- [ ] Add database indexes
- [ ] Configure caching strategies
- [ ] Optimize projection processing
- [ ] Add response compression
- [ ] Implement connection pooling
- [ ] Optimize MediatR pipeline

### Security Hardening
- [ ] Penetration testing
- [ ] Dependency vulnerability scan
- [ ] OWASP compliance check
- [ ] Secrets audit
- [ ] API security review

---

## Phase 9: Pre-Production (Week 9)

### Load Testing
- [ ] Define performance baselines
- [ ] Run load tests (JMeter/K6)
  - [ ] Normal load
  - [ ] Peak load
  - [ ] Stress test
  - [ ] Soak test
- [ ] Analyze bottlenecks
- [ ] Optimize based on results

### Disaster Recovery
- [ ] Test backup procedures
- [ ] Test restore procedures
- [ ] Document RTO/RPO
- [ ] Test failover scenarios
- [ ] Create incident response plan

### User Acceptance Testing
- [ ] Create UAT environment
- [ ] Prepare test data
- [ ] Conduct UAT with stakeholders
- [ ] Fix identified issues
- [ ] Get sign-off

---

## Phase 10: Production Launch (Week 10)

### Pre-Launch
- [ ] Final security review
- [ ] Performance verification
- [ ] Monitoring setup verification
- [ ] Backup verification
- [ ] Rollback plan review
- [ ] Communication plan
- [ ] Support team training

### Launch
- [ ] Deploy to production
- [ ] Monitor for issues
- [ ] Run smoke tests
- [ ] Verify integrations
- [ ] Check monitoring dashboards
- [ ] Notify stakeholders

### Post-Launch
- [ ] Monitor for 48 hours
- [ ] Collect feedback
- [ ] Document lessons learned
- [ ] Plan iteration 2 features
- [ ] Schedule retrospective

---

## Success Criteria

### Performance
- [ ] Command processing < 100ms (p95)
- [ ] Query response time < 50ms (p95)
- [ ] Event processing lag < 1 second
- [ ] Error rate < 0.1%
- [ ] 99.95% uptime

### Quality
- [ ] Domain layer coverage > 95%
- [ ] Application layer coverage > 85%
- [ ] Zero critical bugs
- [ ] All acceptance criteria met
- [ ] MediatR integration validated

### Operations
- [ ] Monitoring in place
- [ ] Alerts configured
- [ ] Runbooks complete
- [ ] On-call rotation established
- [ ] Support documentation ready

---

## NuGet Packages Reference

### Core Packages
```xml
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
```

### Testing Packages
```xml
<PackageReference Include="xUnit" Version="2.6.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Moq" Version="4.20.0" />
<PackageReference Include="Testcontainers" Version="3.6.0" />
```

---

## Team Assignments

### Backend Developers (3)
- Developer 1: Domain & Application Layer (MediatR handlers)
- Developer 2: Infrastructure Layer
- Developer 3: API Layer & Integration

### DevOps Engineer (1)
- CI/CD pipeline
- Infrastructure as Code
- Kubernetes configuration
- Monitoring setup

### QA Engineer (1)
- Test strategy execution
- Automation framework
- Performance testing
- UAT coordination

### Architect (1)
- Technical oversight
- Code reviews
- Architecture decisions
- Technical debt management

---

**Estimated Timeline:** 10 weeks  
**Team Size:** 6 people  
**Estimated Effort:** 240 person-days  
**MediatR Version:** 12.4.1 (Free)  
**Last Updated:** December 2024
