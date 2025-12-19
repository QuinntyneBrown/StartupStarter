# Copilot Instructions for StartupStarter

## Project Architecture
- **Domain-Driven Design (DDD):** Each major business area is a bounded context (e.g., Account Management, User Management, etc.), with clear boundaries and event-driven integration.
- **CQRS & Event Sourcing:** Command and Query sides are separated. All state changes are captured as events and persisted in an event store (EventStoreDB or Cosmos DB). State is rebuilt from event history.
- **MediatR v12.x:** All commands, queries, and domain events are handled via MediatR. Handlers are registered in DI using `AddMediatR`.
- **Clean Architecture:** Four layers—Domain, Application, Infrastructure, Presentation. Inner layers do not depend on outer layers.

## Key Patterns & Conventions
- **Commands/Queries:** Use `IRequestHandler<TRequest, TResponse>` for commands and queries. Example: `CreateAccountHandler`, `GetAccountByIdHandler`.
- **Domain Events:** Use `INotificationHandler<TNotification>` for domain events. Example: `AccountCreatedHandler`.
- **Value Objects:** Use immutable types for identifiers (AccountId, UserId, etc.).
- **Event Naming:** See `docs/events/README.md` and event markdown files for naming and documentation of all event types.
- **Testing:** xUnit/NUnit for unit/integration tests, Playwright for E2E. MediatR handlers are the main test target.

## Developer Workflows
- **Build:** .NET 8.0, standard `dotnet build`.
- **Test:** `dotnet test` for unit/integration. E2E via Playwright.
- **Run:** Standard ASP.NET Core launch. Use Docker for local infra (see deployment docs).
- **CI/CD:** Automated pipeline with build, test, Docker packaging, and blue-green deployment.

## Integration & External Dependencies
- **Azure Services:** Cosmos DB, Service Bus, Redis, AKS, Application Insights.
- **Authentication:** Azure AD B2C / OAuth 2.0.
- **Secrets:** Azure Key Vault.

## Where to Find Key Info
- **Event Definitions:** `docs/events/` (one file per event category)
- **Domain/Architecture:** `docs/detailed-design/account-management/README.md` (patterns, diagrams, business rules)
- **Testing Strategy:** `docs/detailed-design/account-management/10-testing-strategy.puml`
- **Deployment:** `docs/detailed-design/account-management/09-deployment-architecture.puml`

## Project-Specific Practices
- **All business logic is event-driven and MediatR-based.**
- **No direct DB access in domain/application layers.**
- **All changes are events; projections update read models.**
- **Use Clean Architecture boundaries for all new code.**

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

---

For more, see the detailed design docs and event markdowns. Ask the architecture team for any unclear conventions.