# StartupStarter Features Documentation

This directory contains comprehensive documentation for all features in the StartupStarter system.

## Structure

Each feature folder contains:
1. **events.md** - Domain events fired by the feature
2. **models.md** - C# models, entities, enums, and domain events needed to implement the feature
3. **detailed-design/** - Detailed design documentation including:
   - README.md - Comprehensive design overview
   - PlantUML diagrams (where applicable)
   - Sequence diagrams
   - Architecture diagrams

## Features

### 1. [Account Management](account-management/)
Manages tenant accounts (organizations) in the system.
- Account lifecycle (create, update, suspend, delete)
- Subscription tier management
- Account ownership transfer
- Account-level settings

**Key Files:**
- [events.md](account-management/events.md)
- [models.md](account-management/models.md)
- [detailed-design/](account-management/detailed-design/)

### 2. [API Management](api/)
API keys, webhooks, and API request tracking.
- API key generation and management
- Webhook registration and delivery
- API request logging
- Rate limiting

**Key Files:**
- [events.md](api/events.md)
- [models.md](api/models.md)
- [detailed-design/](api/detailed-design/)

### 3. [Audit](audit/)
Comprehensive audit logging for compliance.
- Immutable audit trail
- Export functionality
- Retention policies
- Compliance reporting

**Key Files:**
- [events.md](audit/events.md)
- [models.md](audit/models.md)
- [detailed-design/](audit/detailed-design/)

### 4. [Authentication](authentication/)
User authentication, sessions, and security.
- Login/Logout
- Multi-factor authentication (MFA)
- Session management
- Password reset

**Key Files:**
- [events.md](authentication/events.md)
- [models.md](authentication/models.md)
- [detailed-design/](authentication/detailed-design/)

### 5. [Content Management](content/)
CMS functionality for creating and publishing content.
- Content creation and editing
- Version control
- Draft/Review/Published workflow
- Scheduled publishing

**Key Files:**
- [events.md](content/events.md)
- [models.md](content/models.md)
- [detailed-design/](content/detailed-design/)

### 6. [Dashboard Management](dashboard-management/)
Customizable dashboards with drag-drop functionality.
- Dashboard creation and layout
- Dashboard cards (widgets)
- Card positioning
- Dashboard sharing
- Templates

**Key Files:**
- [events.md](dashboard-management/events.md)
- [models.md](dashboard-management/models.md)
- [detailed-design/](dashboard-management/detailed-design/)

### 7. [Media Management](media/)
Media file upload, storage, and processing.
- File upload (images, videos, documents)
- Cloud storage integration
- Image/Video processing
- Tagging and categorization

**Key Files:**
- [events.md](media/events.md)
- [models.md](media/models.md)
- [detailed-design/](media/detailed-design/)

### 8. [Profile Management](profile-management/)
User profiles within accounts.
- Multiple profiles per account
- Profile preferences
- Avatar management
- Profile sharing

**Key Files:**
- [events.md](profile-management/events.md)
- [models.md](profile-management/models.md)
- [detailed-design/](profile-management/detailed-design/)

### 9. [Role Management](role-management/)
Role-Based Access Control (RBAC).
- Custom roles
- Permission management
- Role assignment
- Permission checking

**Key Files:**
- [events.md](role-management/events.md)
- [models.md](role-management/models.md)
- [detailed-design/](role-management/detailed-design/)

### 10. [System Management](system/)
System-level operations and monitoring.
- Maintenance scheduling
- Automated backups
- Error tracking
- Performance monitoring

**Key Files:**
- [events.md](system/events.md)
- [models.md](system/models.md)
- [detailed-design/](system/detailed-design/)

### 11. [User Management](user-management/)
User account lifecycle management.
- User creation via invitation
- Account activation/deactivation
- User locking
- Account deletion

**Key Files:**
- [events.md](user-management/events.md)
- [models.md](user-management/models.md)
- [detailed-design/](user-management/detailed-design/)

### 12. [Workflow Management](workflow/)
Approval workflow system.
- Multi-stage approvals
- Task assignment
- Approval/rejection with comments
- SLA tracking

**Key Files:**
- [events.md](workflow/events.md)
- [models.md](workflow/models.md)
- [detailed-design/](workflow/detailed-design/)

## Implementation Architecture

The system follows Clean Architecture / Domain-Driven Design principles:

### Core Layer (`StartupStarter.Core`)
```
StartupStarter.Core/
├── Model/
│   ├── {Feature}Aggregate/
│   │   ├── Entities/
│   │   │   ├── {AggregateRoot}.cs
│   │   │   └── {Entity}.cs
│   │   ├── Enums/
│   │   │   └── {Enum}.cs
│   │   └── Events/
│   │       └── {DomainEvent}.cs
│   └── ...
└── Services/
    └── {Service}.cs
```

### Infrastructure Layer (`StartupStarter.Infrastructure`)
```
StartupStarter.Infrastructure/
├── StartupStarterContext.cs (IStartupStarterContext implementation)
├── EntityConfigurations/
│   └── {Entity}Configuration.cs
└── Migrations/
```

### API Layer (`StartupStarter.Api`)
```
StartupStarter.Api/
├── Controllers/
│   └── {Feature}Controller.cs
├── Features/
│   └── {Feature}/
│       ├── Commands/
│       │   └── {Command}.cs
│       └── Queries/
│           └── {Query}.cs
├── Dtos/
│   └── {Feature}Dto.cs
├── Extensions/
│   └── {Feature}Extensions.cs (ToDto methods)
└── Behaviors/
    └── {Behavior}.cs
```

## Key Design Principles

1. **No AutoMapper** - Use extension methods with ToDto() in API layer
2. **No IRepositories** - Use IStartupStarterContext interface directly
3. **Named IDs** - All IDs include entity name (e.g., `AccountId`, not `Id`)
4. **Domain Events** - All aggregates publish events for important state changes
5. **Aggregate Boundaries** - Each aggregate is a consistency boundary
6. **EF Core** - Entity Framework Core for data access
7. **MediatR** - CQRS pattern with MediatR for commands and queries

## Getting Started

1. Review the [models.md](account-management/models.md) for any feature to understand the domain model
2. Check [events.md](account-management/events.md) to see what events are fired
3. Read [detailed-design/README.md](account-management/detailed-design/README.md) for implementation details
4. Review PlantUML diagrams in detailed-design folders for visual architecture

## Dependencies Between Features

```
┌─────────────────────────────────────────────────────┐
│                  Account Management                  │
│                   (Core Boundary)                    │
└───────────────┬─────────────────────────────────────┘
                │
        ┌───────┴────────┬─────────────┬──────────────┐
        ▼                ▼             ▼              ▼
  ┌──────────┐   ┌──────────────┐  ┌──────┐    ┌──────────┐
  │   User   │   │   Profile    │  │ API  │    │   Role   │
  │  Mgmt    │   │     Mgmt     │  │ Mgmt │    │   Mgmt   │
  └────┬─────┘   └──────┬───────┘  └───┬──┘    └─────┬────┘
       │                │              │             │
       │         ┌──────┴──────┐       │             │
       │         ▼             ▼       │             │
       │  ┌──────────┐   ┌──────────┐ │             │
       │  │Dashboard │   │ Content  │◄┴─────────────┘
       │  │   Mgmt   │   │   Mgmt   │
       │  └──────────┘   └────┬─────┘
       │                      │
       │                 ┌────┴─────┐
       │                 ▼          ▼
       │            ┌────────┐  ┌──────────┐
       └───────────►│  Auth  │  │Workflow  │
                    └────────┘  └──────────┘

┌──────────────────────────────────────────────────────┐
│              Cross-Cutting Concerns                   │
│  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐    │
│  │ Audit  │  │ Media  │  │System  │  │  Logs  │    │
│  └────────┘  └────────┘  └────────┘  └────────┘    │
└──────────────────────────────────────────────────────┘
```

## Next Steps

1. Implement each feature following the models.md specifications
2. Create database migrations for entity configurations
3. Implement command and query handlers using MediatR
4. Create API controllers
5. Implement domain event handlers
6. Add integration tests
7. Set up CI/CD pipeline
