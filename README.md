
# StartupStarter

## Overview
StartupStarter is a modular, event-driven platform built using modern .NET practices, Domain-Driven Design (DDD), CQRS, and Event Sourcing. The architecture is designed for scalability, maintainability, and clear separation of concerns.

## Architecture
- **Domain-Driven Design (DDD):** Each business area is a bounded context (e.g., Account Management, User Management, etc.), with clear boundaries and event-driven integration.
- **CQRS & Event Sourcing:** Command and Query responsibilities are separated. All state changes are captured as events and persisted in an event store (EventStoreDB or Cosmos DB). State is rebuilt from event history.
- **MediatR:** All commands, queries, and domain events are handled via MediatR. Handlers are registered in DI using `AddMediatR`.
- **Clean Architecture:** Four layers—Domain, Application, Infrastructure, Presentation. Inner layers do not depend on outer layers.

## Key Patterns & Conventions
- **Commands/Queries:** Use `IRequestHandler<TRequest, TResponse>` for commands and queries. Example: `CreateAccountHandler`, `GetAccountByIdHandler`.
- **Domain Events:** Use `INotificationHandler<TNotification>` for domain events. Example: `AccountCreatedHandler`.
- **Value Objects:** Use immutable types for identifiers (AccountId, UserId, etc.).
- **Event Naming:** See `docs/events/README.md` and event markdown files for naming and documentation of all event types.
- **Testing:** xUnit/NUnit for unit/integration tests, Playwright for E2E. MediatR handlers are the main test target.

## Project Structure
- **src/**: Main source code
	- **StartupStarter.Api/**: ASP.NET Core API, controllers, and feature folders
	- **StartupStarter.Core/**: Domain models, value objects, domain events
	- **StartupStarter.Infrastructure/**: Data access, external integrations, dependency injection
- **docs/**: Architecture, event definitions, business rules, and detailed design for each bounded context

## Developer Workflows
- **Build:** .NET 8.0, standard `dotnet build`.
- **Test:** `dotnet test` for unit/integration. E2E via Playwright.
- **Run:** Standard ASP.NET Core launch. Use Docker for local infra (see deployment docs).
- **CI/CD:** Automated pipeline with build, test, Docker packaging, and blue-green deployment.

## Integration & External Dependencies
- **Azure Services:** Cosmos DB, Service Bus, Redis, AKS, Application Insights.
- **Authentication:** Azure AD B2C / OAuth 2.0.
- **Secrets:** Azure Key Vault.

## Project-Specific Practices
- All business logic is event-driven and MediatR-based.
- No direct DB access in domain/application layers.
- All changes are events; projections update read models.
- Use Clean Architecture boundaries for all new code.

## Example: MediatR Command Handler
```csharp
public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResult>
{
		public async Task<CreateAccountResult> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
		{
				// Implementation
		}
}
```

## Documentation
- **Event Definitions:** `docs/events/` (one file per event category)
- **Domain/Architecture:** `docs/detailed-design/account-management/README.md` (patterns, diagrams, business rules)
- **Testing Strategy:** `docs/detailed-design/account-management/10-testing-strategy.puml`
- **Deployment:** `docs/detailed-design/account-management/09-deployment-architecture.puml`

---
For more, see the detailed design docs and event markdowns. Contact the architecture team for any unclear conventions.

## Project Status

✅ **Project Structure Created**
✅ **AccountAggregate Fully Implemented** (Template for all features)
✅ **Comprehensive Documentation Complete**
✅ **Build Verified**

## Quick Start

```bash
# Build the solution
dotnet build StartupStarter.sln

# The Core layer builds successfully with AccountAggregate as a complete example
```

## Documentation

📖 **[IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md)** - Detailed implementation guide and next steps
📖 **[docs/features/README.md](docs/features/README.md)** - Complete features documentation
📖 **[docs/features/account-management/](docs/features/account-management/)** - Example feature with models, events, and detailed design

## What's Implemented

### ✅ Complete Foundation
- Three-tier architecture (Core, Infrastructure, API)
- NuGet packages configured
- Project references set up
- DomainEvent base class
- IStartupStarterContext interface

### ✅ Three Complete Aggregates (Templates)
**AccountAggregate:**
- Account and AccountSettings entities
- All enums (AccountType, AccountStatus, DeletionType)
- All 10 domain events
- Full business logic and validation

**UserAggregate:**
- User and UserInvitation entities
- All enums (UserStatus, ActivationMethod)
- All 11 domain events
- Complete lifecycle management

**ProfileAggregate:**
- Profile, ProfilePreferences, ProfileShare entities
- All enums (ProfileType, PermissionLevel)
- All 7 domain events
- Dashboard association management

**IStartupStarterContext:**
- ✅ All implemented aggregates registered
- ✅ Ready for Infrastructure layer implementation

## Architecture

```
src/
├── StartupStarter.Core/           # ✅ Foundation complete, AccountAggregate 100%
├── StartupStarter.Infrastructure/ # ⏳ Ready for implementation
└── StartupStarter.Api/            # ⏳ Ready for implementation
```

## Next Steps

See [IMPLEMENTATION_STATUS.md](IMPLEMENTATION_STATUS.md) for:
- Complete implementation guide
- Code examples for Infrastructure and API layers
- Step-by-step instructions for remaining 11 features
- Database migration setup
- Testing strategy

## Technology Stack

- .NET 8
- Entity Framework Core 8
- MediatR 12
- SQL Server
- ASP.NET Core Web API