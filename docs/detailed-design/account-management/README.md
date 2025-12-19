# Account Management Bounded Context - Design Documentation

## Overview

This documentation provides comprehensive design artifacts for implementing the Account Management bounded context using Domain-Driven Design (DDD), Event Sourcing, and CQRS patterns with **MediatR v12.x (latest free version)**.

**Target Audience:**
- Software Architects
- Backend Developers
- QA Engineers
- Business Analysts
- Product Managers

---

## 📋 Table of Contents

1. [Domain Model](#domain-model)
2. [CQRS Architecture](#cqrs-architecture)
3. [Command Flow](#command-flow)
4. [Query Flow](#query-flow)
5. [Event Sourcing Pattern](#event-sourcing-pattern)
6. [Clean Architecture](#clean-architecture)
7. [Bounded Context Integration](#bounded-context-integration)
8. [State Machine](#state-machine)
9. [Deployment Architecture](#deployment-architecture)
10. [Testing Strategy](#testing-strategy)

---

## 🎯 Business Context

The Account Management bounded context is responsible for managing organizational accounts (tenants) in the Contently Headless CMS platform. 

**Key Responsibilities:**
- Account lifecycle management (create, update, suspend, delete)
- Subscription tier management
- Account ownership and settings
- Profile association within accounts
- Account status and access control

**Business Rules:**
- Each User has exactly one Account
- An Account can have many Profiles
- Account status determines access rights
- Subscription changes affect available features
- Account deletion cascades to profiles and content

---

## 📊 Diagram Descriptions

### 1. Domain Model (01-domain-model.puml)

**Purpose:** Shows the core domain entities, value objects, and domain events using Event Sourcing.

**For Developers:**
- Complete aggregate structure with properties and methods
- Event sourcing methods (Apply, LoadFromHistory)
- All domain events with payloads
- Business rule enforcement points

**For Business:**
- Main business entity (Account)
- Account types and statuses
- Key operations available
- Data captured in each event

**Key Concepts:**
- **Account Aggregate Root:** Main entity managing account lifecycle
- **Value Objects:** AccountId, UserId, ProfileId (immutable identifiers)
- **Domain Events:** All changes represented as events
- **Event Sourcing:** State rebuilt from event history

---

### 2. CQRS Architecture (02-cqrs-architecture.puml)

**Purpose:** Illustrates the Command Query Responsibility Segregation pattern separating writes from reads using **MediatR v12.x**.

**For Developers:**
- Command side (write model) architecture
- Query side (read model) architecture
- Event flow from commands to projections
- Infrastructure components
- **MediatR v12.x integration pattern**

**For Business:**
- Commands change the system (e.g., create account)
- Queries retrieve data (e.g., get account details)
- Different optimization strategies for each
- Eventually consistent reads

**MediatR v12.x Key Interfaces:**
- `IRequestHandler<TRequest, TResponse>` - For commands and queries
- `INotificationHandler<TNotification>` - For domain events
- `ISender` - For sending requests
- `IPublisher` - For publishing notifications

**Key Benefits:**
- **Scalability:** Read and write sides scale independently
- **Performance:** Optimized read models for queries
- **Flexibility:** Multiple read models for different use cases
- **Simplicity:** Clear separation of concerns

---

### 3. Command Flow (03-create-account-flow.puml)

**Purpose:** Detailed sequence diagram showing how a create account command flows through the system using **MediatR**.

**For Developers:**
- Step-by-step command processing with MediatR
- Validation points
- Event store interaction
- Async event publishing
- Projection updates

**For Business:**
- How long operations take
- Where validation occurs
- When users get responses
- How data becomes queryable

**Flow Steps:**
1. User sends command via API
2. Controller uses `ISender.Send(command)`
3. MediatR dispatches to appropriate handler
4. Command validated and authorized
5. Aggregate processes business logic
6. Events persisted to event store
7. Events published to message bus
8. Projections update read models
9. Data available for queries

---

### 4. Query Flow (04-query-account-flow.puml)

**Purpose:** Shows how queries retrieve data from optimized read models using **MediatR**.

**For Developers:**
- Simple query processing with MediatR
- Read model access
- No domain logic needed
- Fast response times

**For Business:**
- Queries are fast (sub-second)
- No impact on write operations
- Can have multiple views of same data
- Data may be slightly stale (eventual consistency)

---

### 5. Event Sourcing Pattern (05-event-sourcing-pattern.puml)

**Purpose:** Explains how Event Sourcing works with detailed examples.

**For Developers:**
- Event stream structure
- State reconstruction from events
- Snapshot optimization
- Versioning and concurrency

**For Business:**
- Complete audit trail of all changes
- Time travel capability (see past states)
- Can replay events to fix bugs
- Business intelligence opportunities

**Key Benefits:**
- **Audit Trail:** Every change recorded
- **Temporal Queries:** Answer "what was the state on date X?"
- **Event Replay:** Fix data issues by replaying
- **Analytics:** Rich event data for reporting

---

### 6. Clean Architecture (06-clean-architecture.puml)

**Purpose:** Shows the layered architecture following Clean Architecture principles with **MediatR v12.x** integration.

**For Developers:**
- Layer dependencies (inner layers don't depend on outer)
- Component organization
- Interface definitions
- Testability boundaries
- **MediatR registration and usage**

**For Business:**
- Modular, maintainable codebase
- Easy to change infrastructure
- Business logic protected from technical changes
- Long-term sustainability

**Layers:**
1. **Domain Layer:** Pure business logic
2. **Application Layer:** Use case orchestration (MediatR handlers)
3. **Infrastructure Layer:** Technical implementations
4. **Presentation Layer:** API and UI concerns

---

### 7. Bounded Context Integration (07-bounded-context-integration.puml)

**Purpose:** Shows how Account Management integrates with other bounded contexts.

**For Developers:**
- Event-driven integration pattern
- Loose coupling between contexts
- Event subscriptions
- Integration points

**For Business:**
- How systems work together
- Impact of account changes on other systems
- Notifications and automations triggered
- Data consistency across systems

**Integrated Systems:**
- User Management (owner validation)
- Profile Management (profile lifecycle)
- Billing (subscription management)
- Audit Log (compliance tracking)
- Notifications (email alerts)

---

### 8. State Machine (08-state-machine.puml)

**Purpose:** Visualizes the account lifecycle and valid state transitions.

**For Developers:**
- Valid state transitions
- Business rules per state
- Event triggers for transitions
- Invariants enforced

**For Business:**
- Account lifecycle stages
- What can be done in each state
- How accounts move between states
- Deletion and suspension rules

**States:**
- **Active:** Normal operations, all features available
- **Suspended:** Limited access, pending resolution
- **Deleted:** End state, data archived/removed

---

### 9. Deployment Architecture (09-deployment-architecture.puml)

**Purpose:** Shows the cloud infrastructure and deployment strategy.

**For Developers:**
- Azure services used
- Kubernetes configuration
- Database choices
- Scaling strategies
- Monitoring and security

**For Business:**
- High availability (99.95%+ uptime)
- Automatic scaling
- Global distribution capability
- Cost optimization
- Security and compliance

**Infrastructure:**
- Azure Kubernetes Service (container orchestration)
- Cosmos DB (global database)
- Service Bus (messaging)
- Redis (caching)
- Application Insights (monitoring)

---

### 10. Testing Strategy (10-testing-strategy.puml)

**Purpose:** Comprehensive testing approach following the test pyramid.

**For Developers:**
- Test categories and tools
- Coverage targets
- CI/CD integration
- Test data management
- **Testing MediatR handlers**

**For QA Engineers:**
- What to test at each level
- Test automation strategy
- Performance testing approach
- Contract testing

**For Business:**
- Quality assurance process
- Fast feedback loops
- Risk mitigation
- Confidence in releases

**Test Levels:**
1. **Unit Tests (500+):** Fast, isolated domain logic tests
2. **Integration Tests (200+):** Component interaction tests
3. **E2E Tests (50+):** Full workflow tests
4. **Contract Tests (30+):** API compatibility tests

---

## 🏗️ Implementation Guidelines

### Technology Stack Recommendation

**Backend:**
- .NET 8.0 (C#)
- ASP.NET Core Web API
- **MediatR v12.4.1 (latest free version)**
  - `Install-Package MediatR`
  - `Install-Package MediatR.Extensions.Microsoft.DependencyInjection`
- EventStoreDB or Cosmos DB (Event Store)
- Cosmos DB (Read Models)
- Azure Service Bus (Event Bus)

**Infrastructure:**
- Azure Kubernetes Service (AKS)
- Azure Cosmos DB
- Azure Service Bus
- Azure Redis Cache
- Azure Monitor / Application Insights

**Testing:**
- xUnit / NUnit
- FluentAssertions
- Moq / NSubstitute
- Testcontainers
- Playwright (E2E)

---

## 📈 Key Patterns and Practices

### MediatR v12.x Integration

**Setup in Program.cs:**
```csharp
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
```

**Command Handler Example:**
```csharp
public class CreateAccountHandler 
    : IRequestHandler<CreateAccountCommand, CreateAccountResult>
{
    public async Task<CreateAccountResult> Handle(
        CreateAccountCommand request, 
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**Query Handler Example:**
```csharp
public class GetAccountByIdHandler 
    : IRequestHandler<GetAccountByIdQuery, AccountDto>
{
    public async Task<AccountDto> Handle(
        GetAccountByIdQuery request, 
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**Domain Event Notification:**
```csharp
public class AccountCreatedNotification : INotification
{
    public string AccountId { get; set; }
    // Other properties
}

public class AccountCreatedHandler 
    : INotificationHandler<AccountCreatedNotification>
{
    public async Task Handle(
        AccountCreatedNotification notification, 
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Domain-Driven Design (DDD)
- **Ubiquitous Language:** Business terms in code
- **Bounded Context:** Clear boundaries
- **Aggregate Root:** Transaction boundary (Account)
- **Value Objects:** Immutable identifiers
- **Domain Events:** First-class citizens

### Event Sourcing
- **Event Store:** Append-only event log
- **State Reconstruction:** Replay events to rebuild state
- **Snapshots:** Performance optimization
- **Versioning:** Handle schema evolution

### CQRS with MediatR
- **Command Side:** Optimized for writes (IRequestHandler)
- **Query Side:** Optimized for reads (IRequestHandler)
- **Eventual Consistency:** Acceptable delay
- **Multiple Read Models:** Different views
- **Notifications:** Domain events (INotificationHandler)

### Clean Architecture
- **Dependency Rule:** Inner layers don't depend on outer
- **Domain Purity:** No infrastructure in domain
- **Testability:** Easy to test in isolation
- **Flexibility:** Easy to change infrastructure

---

## 🔐 Security Considerations

1. **Authentication:** Azure AD B2C / OAuth 2.0
2. **Authorization:** Role-based and policy-based
3. **Data Protection:** Encryption at rest and in transit
4. **API Security:** Rate limiting, input validation
5. **Secrets Management:** Azure Key Vault
6. **Audit Logging:** All operations logged
7. **GDPR Compliance:** Data deletion capabilities

---

## 📊 Monitoring and Observability

1. **Metrics:**
   - Command processing time
   - Query response time
   - Event processing lag
   - Error rates
   - MediatR pipeline performance

2. **Logs:**
   - Structured logging (Serilog)
   - Correlation IDs for tracing
   - Application Insights integration
   - MediatR handler execution logs

3. **Distributed Tracing:**
   - End-to-end request tracing
   - Performance bottleneck identification

4. **Health Checks:**
   - Liveness probes
   - Readiness probes
   - Dependency health

---

## 🚀 Deployment Strategy

### CI/CD Pipeline
1. **Build:** Compile, unit tests, static analysis
2. **Test:** Integration tests, contract tests
3. **Package:** Docker images
4. **Deploy to Dev:** Automated
5. **Deploy to Staging:** Automated with E2E tests
6. **Deploy to Production:** Blue-green deployment

### Rollout Strategy
- **Blue-Green Deployment:** Zero downtime
- **Feature Flags:** Gradual rollout
- **Canary Releases:** Test with subset of users
- **Rollback Plan:** Automated rollback on errors

---

## 📚 Additional Resources

### Documentation
- API documentation (OpenAPI/Swagger)
- Architecture Decision Records (ADRs)
- Runbooks for operations
- **MediatR Documentation:** https://github.com/jbogard/MediatR

### Training Materials
- Developer onboarding guide
- Domain model walkthrough
- Event storming sessions
- Code examples and samples
- MediatR best practices

---

## 🤝 Contributing

### Code Standards
- Clean Code principles
- SOLID principles
- Domain-Driven Design patterns
- Test-Driven Development (TDD)
- MediatR handler conventions

### Code Review
- All changes reviewed
- Automated checks (CI)
- Architecture compliance
- Security review for sensitive changes

---

## 📞 Support

For questions or clarifications:
- Architecture Team: architecture@contently.com
- Development Team: dev@contently.com
- Documentation: [Confluence/Wiki Link]

---

**Document Version:** 1.1  
**Last Updated:** December 2024  
**MediatR Version:** 12.4.1 (Free)  
**Authors:** Architecture Team  
**Reviewers:** Technical Leadership, Product Management
