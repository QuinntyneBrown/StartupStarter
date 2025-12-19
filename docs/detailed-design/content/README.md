# Content Management - Detailed Design

This folder contains the detailed design documentation for the Content Management feature implementation using Clean Architecture, MediatR, and Angular 21.

## Overview

The Content Management system handles all content lifecycle events including creation, modification, versioning, publishing, and scheduling. The implementation follows Clean Architecture principles with clear separation of concerns across layers.

## Architecture Documents

### 1. Domain Model (`domain-model.puml`)

**PlantUML class diagram** showing:
- **Content** aggregate root with full business logic
- **ContentVersion** entity for version history
- **ContentMetadata** entity for extensible metadata
- Domain events raised by the aggregate
- Value objects (ContentStatus, DeletionType)
- Relationships between entities

**Key Design Patterns:**
- Aggregate Root pattern (DDD)
- Domain Events pattern
- Value Objects for type safety

### 2. Sequence Diagrams (`sequence-diagrams.puml`)

**Comprehensive flow diagrams** for:
- **Create Content**: Full MediatR command flow with validation and event publishing
- **Update Content**: Version control and optimistic concurrency handling
- **Publish Content**: Publishing workflow with Service Bus integration
- **Restore Version**: Version history restoration process
- **Schedule Content**: Background job scheduling for future publication

**Technology Integration:**
- MediatR request/response pipeline
- FluentValidation in pipeline behaviors
- Entity Framework Core with DbContext
- Azure Service Bus for integration events
- SignalR for real-time notifications
- Hangfire/Azure Functions for background jobs

### 3. Clean Architecture Diagram (`clean-architecture.drawio`)

**Draw.io architecture layers** including:

#### Presentation Layer (Angular 21)
- Content List Component
- Content Editor Component
- Version History Component
- Content Service (Angular)
- SignalR client integration

#### API Layer (.NET Core)
- API Gateway with JWT authentication
- ContentController (CRUD operations)
- PublishController (publishing workflows)
- VersionController (version management)
- ContentHub (SignalR real-time hub)

#### Application Layer (MediatR)
- Commands: CreateContent, UpdateContent, PublishContent, ScheduleContent, RestoreVersion
- Queries: GetContentById, GetContentsByAccount, GetContentVersions
- FluentValidation validators
- AutoMapper configurations

#### Domain Layer
- Content aggregate root
- ContentVersion entity
- ContentMetadata entity
- Domain events
- Repository interfaces

#### Infrastructure Layer
- Entity Framework Core DbContext
- Repository implementations
- Domain Event Dispatcher
- SignalR Service
- Background job scheduler

#### External Services
- Azure SQL Database
- Azure Service Bus
- Azure Blob Storage
- Azure Redis Cache
- Application Insights

### 4. Component Diagram (`component-diagram.puml`)

**PlantUML component architecture** showing:
- Component relationships across all layers
- Dependency flow from UI to database
- MediatR pipeline execution
- Event dispatching mechanism
- Integration with external Azure services

## Technology Stack

### Backend
- **.NET 8** - Web API framework
- **MediatR** - CQRS and mediator pattern implementation
- **Entity Framework Core** - ORM and data access
- **FluentValidation** - Request validation
- **AutoMapper** - Object-to-object mapping
- **SignalR** - Real-time communication
- **Hangfire/Azure Functions** - Background job processing

### Frontend
- **Angular 21** - Admin UI framework
- **RxJS** - Reactive state management
- **Angular Material** - UI components
- **TypeScript** - Type-safe development

### Azure Services
- **Azure SQL Database** - Relational data storage
- **Azure Service Bus** - Message broker for integration events
- **Azure Blob Storage** - Media file storage
- **Azure Redis Cache** - Distributed caching
- **Application Insights** - Monitoring and logging

## Implementation Guidelines

### Clean Architecture Principles

1. **Dependency Rule**: Dependencies point inward
   - Presentation → Application → Domain ← Infrastructure
   - Domain has no dependencies on other layers
   - Infrastructure implements interfaces defined in Domain/Application

2. **Separation of Concerns**
   - **Domain**: Business logic and rules
   - **Application**: Use cases and orchestration
   - **Infrastructure**: External concerns (database, messaging)
   - **Presentation**: User interface and API endpoints

3. **Testability**
   - Domain logic is pure and easily testable
   - Application handlers can be unit tested with mocked repositories
   - Integration tests for Infrastructure layer

### MediatR Pattern

```csharp
// Command
public record CreateContentCommand(
    string Title,
    string Body,
    Guid AccountId) : IRequest<CreateContentResponse>;

// Handler
public class CreateContentHandler : IRequestHandler<CreateContentCommand, CreateContentResponse>
{
    private readonly IContentRepository _repository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<CreateContentResponse> Handle(
        CreateContentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create domain entity (raises domain events)
        var content = Content.Create(request.Title, request.Body, request.AccountId);

        // 2. Persist to database
        await _repository.AddAsync(content, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 3. Dispatch domain events (after save)
        await _eventDispatcher.DispatchAsync(content.DomainEvents, cancellationToken);

        // 4. Return response
        return new CreateContentResponse(content.ContentId, content.Status);
    }
}
```

### Domain-Driven Design

**Aggregate Root (Content):**
- Enforces business invariants
- Raises domain events for state changes
- Provides factory methods (e.g., `Content.Create()`)
- Encapsulates all business logic

**Domain Events:**
- Raised by aggregate roots
- Represent state changes
- Dispatched after successful persistence
- Enable event-driven architecture

### Event-Driven Architecture

1. **Domain Events** (internal)
   - Raised within the domain boundary
   - Handled synchronously after SaveChanges
   - Example: `ContentCreatedDomainEvent`

2. **Integration Events** (external)
   - Published to Azure Service Bus
   - Handled asynchronously by other services
   - Example: Content published → trigger CDN cache invalidation

3. **Real-time Events** (SignalR)
   - Sent to connected clients
   - Enable collaborative editing
   - Example: Notify users when content is published

## Key Workflows

### Content Creation Flow
1. User fills form in Angular UI
2. HTTP POST to `/api/content`
3. API Gateway authenticates with JWT
4. ContentController sends `CreateContentCommand` via MediatR
5. CreateContentHandler validates and creates Content aggregate
6. Repository persists to database
7. Domain events dispatched (integration events + SignalR)
8. Response returned to UI

### Content Publishing Flow
1. User clicks "Publish" button
2. HTTP POST to `/api/content/{id}/publish`
3. PublishContentHandler retrieves content
4. Validates content is ready for publishing
5. Calls `content.Publish()` (raises domain event)
6. Persists status change
7. Publishes integration event to Service Bus
8. SignalR notifies connected clients
9. Background services process (cache invalidation, CDN, webhooks)

### Version Restoration Flow
1. User views version history
2. Selects version to restore
3. HTTP POST to `/api/content/{id}/restore-version`
4. RestoreVersionHandler retrieves content and version
5. Creates snapshot of current state
6. Restores data from selected version
7. Increments version number
8. Persists changes
9. Returns success response

### Scheduled Publishing Flow
1. User sets future publish date
2. HTTP POST to `/api/content/{id}/schedule`
3. ScheduleContentHandler validates date
4. Sets ScheduledPublishDate on content
5. Publishes `ContentScheduledEvent` to Service Bus
6. Background job scheduler (Hangfire/Azure Functions) creates job
7. At scheduled time:
   - Job executes `PublishContentCommand`
   - Content status changes to Published
   - Standard publishing workflow executes

## Database Schema

### Content Table
```sql
CREATE TABLE Content (
    ContentId UNIQUEIDENTIFIER PRIMARY KEY,
    ContentType NVARCHAR(100) NOT NULL,
    Title NVARCHAR(500) NOT NULL,
    Body NVARCHAR(MAX),
    AuthorId UNIQUEIDENTIFIER NOT NULL,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    ProfileId UNIQUEIDENTIFIER NULL,
    Status INT NOT NULL,
    CurrentVersionNumber INT NOT NULL,
    PublishDate DATETIME2 NULL,
    ScheduledPublishDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    DeletedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    RowVersion ROWVERSION,

    FOREIGN KEY (AuthorId) REFERENCES Users(UserId),
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    FOREIGN KEY (ProfileId) REFERENCES Profiles(ProfileId)
);

CREATE INDEX IX_Content_AccountId ON Content(AccountId);
CREATE INDEX IX_Content_Status ON Content(Status);
CREATE INDEX IX_Content_IsDeleted ON Content(IsDeleted);
```

### ContentVersions Table
```sql
CREATE TABLE ContentVersions (
    VersionId UNIQUEIDENTIFIER PRIMARY KEY,
    ContentId UNIQUEIDENTIFIER NOT NULL,
    VersionNumber INT NOT NULL,
    Title NVARCHAR(500) NOT NULL,
    Body NVARCHAR(MAX),
    ChangeDescription NVARCHAR(1000),
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL,

    FOREIGN KEY (ContentId) REFERENCES Content(ContentId) ON DELETE CASCADE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),

    UNIQUE (ContentId, VersionNumber)
);

CREATE INDEX IX_ContentVersions_ContentId ON ContentVersions(ContentId);
```

### ContentMetadata Table
```sql
CREATE TABLE ContentMetadata (
    MetadataId UNIQUEIDENTIFIER PRIMARY KEY,
    ContentId UNIQUEIDENTIFIER NOT NULL,
    [Key] NVARCHAR(100) NOT NULL,
    Value NVARCHAR(MAX),
    DataType NVARCHAR(50) NOT NULL,

    FOREIGN KEY (ContentId) REFERENCES Content(ContentId) ON DELETE CASCADE
);

CREATE INDEX IX_ContentMetadata_ContentId ON ContentMetadata(ContentId);
CREATE INDEX IX_ContentMetadata_Key ON ContentMetadata([Key]);
```

## Testing Strategy

### Unit Tests
- Domain entity logic (Content aggregate methods)
- Command/Query handlers with mocked dependencies
- Validators using FluentValidation test helpers

### Integration Tests
- API endpoints with WebApplicationFactory
- Repository implementations with test database
- Event dispatching and SignalR notifications

### E2E Tests
- Complete user workflows in Angular UI
- Content creation → editing → publishing flow
- Version management scenarios

## Security Considerations

### Authentication & Authorization
- JWT Bearer token authentication on all endpoints
- Claims-based authorization (AccountId, UserId)
- Validate user has permission to modify content

### Data Validation
- FluentValidation at application layer
- Domain invariants enforced in aggregates
- SQL injection prevention via parameterized queries

### Concurrency Control
- Optimistic concurrency with RowVersion
- Version number validation on updates
- Conflict detection and resolution

## Performance Optimizations

### Caching Strategy
- Cache frequently accessed content (published)
- Redis distributed cache for multi-instance deployments
- Cache invalidation on content updates

### Database Optimization
- Indexed foreign keys and status columns
- Pagination for list queries
- EF Core query optimization (AsNoTracking for reads)

### Async Operations
- All I/O operations are async
- Background processing for heavy operations
- Service Bus for decoupled integration

## Monitoring & Logging

### Application Insights
- Request/response logging
- Exception tracking
- Custom events for business operations

### Structured Logging
- Serilog with JSON formatting
- Correlation IDs for request tracing
- Log levels: Debug, Information, Warning, Error, Critical

### Metrics
- Content creation rate
- Publishing success rate
- Average response times
- Cache hit ratios

## Next Steps

1. **Implementation**: Follow the model definitions in `/docs/model/content.md`
2. **Database**: Create migrations using EF Core
3. **Testing**: Implement unit and integration tests
4. **Deployment**: Configure Azure resources
5. **Documentation**: API documentation with Swagger/OpenAPI

## References

- Event Definitions: `/docs/events/content.md`
- Model Definitions: `/docs/model/content.md`
- Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- MediatR Documentation: https://github.com/jbogard/MediatR
- Domain-Driven Design: Eric Evans, "Domain-Driven Design"

---

**Last Updated**: December 2024
**Version**: 1.0
**Status**: Design Complete - Ready for Implementation
