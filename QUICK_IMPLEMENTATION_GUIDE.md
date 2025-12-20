# Quick Implementation Guide

## Status Summary

### ✅ Fully Implemented (100%)
1. **AccountAggregate** - Complete with all events, entities, enums
2. **UserAggregate** - Complete with User, UserInvitation, all events

### 🔨 Partially Implemented (30%)
3. **ProfileAggregate** - Profile entity created, needs events and related entities

### ⏳ Structure Created, Needs Implementation
4. RoleAggregate
5. ContentAggregate
6. DashboardAggregate
7. MediaAggregate
8. ApiKeyAggregate
9. WebhookAggregate
10. AuditAggregate
11. AuthenticationAggregate
12. WorkflowAggregate
13. MaintenanceAggregate
14. BackupAggregate
15. SystemErrorAggregate

## Template for Implementing Remaining Aggregates

### Step 1: Create Enums (if needed)

```csharp
// Example: ContentAggregate/Enums/ContentStatus.cs
namespace StartupStarter.Core.Model.ContentAggregate.Enums;

public enum ContentStatus
{
    Draft,
    Review,
    Approved,
    Published,
    Unpublished,
    Archived,
    Deleted
}
```

### Step 2: Create Main Entity (Aggregate Root)

```csharp
// Example Pattern - see Account.cs or User.cs
using StartupStarter.Core.Model.{Feature}Aggregate.Enums;
using StartupStarter.Core.Model.{Feature}Aggregate.Events;

namespace StartupStarter.Core.Model.{Feature}Aggregate.Entities;

public class {Entity}
{
    // Properties with private setters
    public string {Entity}Id { get; private set; }
    // ... other properties

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private {Entity}() { }

    // Public constructor
    public {Entity}(params...)
    {
        // Validation
        // Set properties
        // Raise domain event
        AddDomainEvent(new {Entity}CreatedEvent { ... });
    }

    // Business methods
    public void SomeAction(params...)
    {
        // Business logic
        // Update properties
        // Raise domain event
        AddDomainEvent(new {Entity}UpdatedEvent { ... });
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
```

### Step 3: Create Domain Events

```csharp
// Example: {Feature}Aggregate/Events/{Entity}CreatedEvent.cs
namespace StartupStarter.Core.Model.{Feature}Aggregate.Events;

public class {Entity}CreatedEvent : DomainEvent
{
    public string {Entity}Id { get; set; } = string.Empty;
    // ... other properties from event spec in docs/features/{feature}/events.md
    public DateTime Timestamp { get; set; }
}
```

### Step 4: Add to IStartupStarterContext

```csharp
// In src/StartupStarter.Core/IStartupStarterContext.cs
DbSet<{Entity}> {Entities} { get; set; }
```

## Prioritized Implementation Order

Based on dependencies:

1. ✅ **AccountAggregate** (DONE)
2. ✅ **UserAggregate** (DONE)
3. 🔨 **ProfileAggregate** (Finish events and related entities)
4. **RoleAggregate** (Needed by User)
5. **ContentAggregate** (Core feature)
6. **DashboardAggregate** (Depends on Profile)
7. **MediaAggregate** (Used by Content)
8. **ApiKeyAggregate** & **WebhookAggregate**
9. **AuthenticationAggregate** (Sessions, MFA)
10. **WorkflowAggregate** (For Content approval)
11. **AuditAggregate** (Cross-cutting)
12. **SystemAggregates** (Maintenance, Backup, Error)

## Quick Commands to Generate Files

### For each aggregate, create the events:

```bash
# Navigate to the aggregate Events folder
cd src/StartupStarter.Core/Model/{Feature}Aggregate/Events

# For each event in docs/features/{feature}/events.md, create a file
# Copy the pattern from Account or User events
```

### Example: Create Content Events

Refer to `docs/features/content/events.md` for the event specifications, then create:

- ContentCreatedEvent.cs
- ContentUpdatedEvent.cs
- ContentDeletedEvent.cs
- ContentPublishedEvent.cs
- ContentUnpublishedEvent.cs
- ContentStatusChangedEvent.cs
- ContentVersionCreatedEvent.cs
- ContentVersionRestoredEvent.cs
- ContentScheduledEvent.cs
- ContentScheduleCancelledEvent.cs

## Helper Entity Pattern

For child entities (like AccountSettings, ProfilePreferences):

```csharp
namespace StartupStarter.Core.Model.{Feature}Aggregate.Entities;

public class {ChildEntity}
{
    public string {ChildEntity}Id { get; private set; }
    public string {Parent}Id { get; private set; }
    // ... properties

    public {Parent} {Parent} { get; private set; } = null!;

    private {ChildEntity}() { }

    public {ChildEntity}(params...)
    {
        // Set properties
    }
}
```

## After Core Layer is Complete

### 1. Infrastructure Layer

Create `StartupStarterContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;

namespace StartupStarter.Infrastructure;

public class StartupStarterContext : DbContext, IStartupStarterContext
{
    public StartupStarterContext(DbContextOptions<StartupStarterContext> options)
        : base(options)
    {
    }

    // Add all DbSets
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    // ... all other entities

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StartupStarterContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Publish domain events before saving
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

Create Entity Configurations (see IMPLEMENTATION_STATUS.md for examples).

### 2. API Layer

Create for each feature:
- DTOs
- Extension methods (ToDto)
- Commands/Queries
- Handlers
- Controller

See IMPLEMENTATION_STATUS.md for complete examples.

## Build and Test

```bash
# Build Core
cd src/StartupStarter.Core
dotnet build

# Once all aggregates are done, build entire solution
cd ../..
dotnet build

# Create migration
cd src/StartupStarter.Api
dotnet ef migrations add InitialCreate --project ../StartupStarter.Infrastructure

# Update database
dotnet ef database update
```

## Reference Documentation

- **Models**: `docs/features/{feature}/models.md`
- **Events**: `docs/features/{feature}/events.md`
- **Design**: `docs/features/{feature}/detailed-design/README.md`
- **Implementation**: `IMPLEMENTATION_STATUS.md`

## Time Estimates

- Simple aggregate (Role): 30-45 minutes
- Medium aggregate (Profile, Content): 1-2 hours
- Complex aggregate (Dashboard, Workflow): 2-3 hours

Total estimated time to complete all Core aggregates: 15-20 hours

Once Core is complete, Infrastructure and API layers will be faster as they follow mechanical patterns.
