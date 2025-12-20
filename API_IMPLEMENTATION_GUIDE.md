# API Layer Implementation Guide

## Status: API Foundation Complete

### ✅ Completed Components

1. **Project Configuration**
   - MediatR configured and registered
   - Controllers enabled
   - Infrastructure services integrated
   - Connection string configured

2. **Account Management Feature (Complete Example)**
   - ✅ DTOs with ToDto extension methods
   - ✅ Commands with Handlers (CreateAccount)
   - ✅ Queries with Handlers (GetAccountById)
   - ✅ Controller (AccountsController)

### 📁 Project Structure

```
StartupStarter.Api/
├── Controllers/
│   └── AccountsController.cs ✅
├── Features/
│   └── AccountManagement/ ✅
│       ├── Commands/
│       │   ├── CreateAccountCommand.cs
│       │   └── CreateAccountCommandHandler.cs
│       ├── Queries/
│       │   ├── GetAccountByIdQuery.cs
│       │   └── GetAccountByIdQueryHandler.cs
│       └── Dtos/
│           ├── AccountDto.cs
│           └── AccountExtensions.cs
└── Program.cs ✅
```

---

## Implementation Pattern

### 1. Create Feature Folder Structure

For each feature (e.g., UserManagement, ProfileManagement):

```bash
cd src/StartupStarter.Api/Features
mkdir {FeatureName}
cd {FeatureName}
mkdir Commands Queries Dtos
```

### 2. Create DTOs with Extension Methods

**File**: `Features/{FeatureName}/Dtos/{Entity}Dto.cs`

```csharp
namespace StartupStarter.Api.Features.{FeatureName}.Dtos;

public class {Entity}Dto
{
    public string {Entity}Id { get; set; } = string.Empty;
    // ... other properties matching the entity
}
```

**File**: `Features/{FeatureName}/Dtos/{Entity}Extensions.cs`

```csharp
using StartupStarter.Core.Model.{Aggregate}Aggregate.Entities;

namespace StartupStarter.Api.Features.{FeatureName}.Dtos;

public static class {Entity}Extensions
{
    public static {Entity}Dto ToDto(this {Entity} entity)
    {
        return new {Entity}Dto
        {
            {Entity}Id = entity.{Entity}Id,
            // ... map all properties
        };
    }
}
```

### 3. Create Commands

**File**: `Features/{FeatureName}/Commands/Create{Entity}Command.cs`

```csharp
using MediatR;
using StartupStarter.Api.Features.{FeatureName}.Dtos;

namespace StartupStarter.Api.Features.{FeatureName}.Commands;

public class Create{Entity}Command : IRequest<{Entity}Dto>
{
    // Properties for creating the entity
    public string PropertyName { get; set; } = string.Empty;
}
```

**File**: `Features/{FeatureName}/Commands/Create{Entity}CommandHandler.cs`

```csharp
using MediatR;
using StartupStarter.Api.Features.{FeatureName}.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.{Aggregate}Aggregate.Entities;

namespace StartupStarter.Api.Features.{FeatureName}.Commands;

public class Create{Entity}CommandHandler : IRequestHandler<Create{Entity}Command, {Entity}Dto>
{
    private readonly IStartupStarterContext _context;

    public Create{Entity}CommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<{Entity}Dto> Handle(Create{Entity}Command request, CancellationToken cancellationToken)
    {
        var entityId = Guid.NewGuid().ToString();

        var entity = new {Entity}(
            entityId,
            request.PropertyName
            // ... other constructor parameters
        );

        _context.{Entities}.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
```

### 4. Create Queries

**File**: `Features/{FeatureName}/Queries/Get{Entity}ByIdQuery.cs`

```csharp
using MediatR;
using StartupStarter.Api.Features.{FeatureName}.Dtos;

namespace StartupStarter.Api.Features.{FeatureName}.Queries;

public class Get{Entity}ByIdQuery : IRequest<{Entity}Dto?>
{
    public string {Entity}Id { get; set; } = string.Empty;
}
```

**File**: `Features/{FeatureName}/Queries/Get{Entity}ByIdQueryHandler.cs`

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.{FeatureName}.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.{FeatureName}.Queries;

public class Get{Entity}ByIdQueryHandler : IRequestHandler<Get{Entity}ByIdQuery, {Entity}Dto?>
{
    private readonly IStartupStarterContext _context;

    public Get{Entity}ByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<{Entity}Dto?> Handle(Get{Entity}ByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.{Entities}
            .FirstOrDefaultAsync(e => e.{Entity}Id == request.{Entity}Id, cancellationToken);

        return entity?.ToDto();
    }
}
```

### 5. Create Controller

**File**: `Controllers/{Entities}Controller.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.{FeatureName}.Commands;
using StartupStarter.Api.Features.{FeatureName}.Dtos;
using StartupStarter.Api.Features.{FeatureName}.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class {Entities}Controller : ControllerBase
{
    private readonly IMediator _mediator;

    public {Entities}Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<{Entity}Dto>> Create{Entity}([FromBody] Create{Entity}Command command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get{Entity}), new { id = result.{Entity}Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<{Entity}Dto>> Get{Entity}(string id)
    {
        var query = new Get{Entity}ByIdQuery { {Entity}Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
```

---

## Features to Implement

Following the pattern above, implement these features:

### Priority 1: Core Features
1. **UserManagement**
   - Users, UserInvitations
   - Reference: `docs/features/user-management/`

2. **ProfileManagement**
   - Profiles, ProfilePreferences, ProfileShare
   - Reference: `docs/features/profile-management/`

3. **RoleManagement**
   - Roles, UserRoles
   - Reference: `docs/features/role-management/`

### Priority 2: Content & Dashboard
4. **ContentManagement**
   - Content, ContentVersions
   - Reference: `docs/features/content/`

5. **DashboardManagement**
   - Dashboards, DashboardCards, DashboardShares
   - Reference: `docs/features/dashboard-management/`

### Priority 3: Media & API
6. **MediaManagement**
   - Media
   - Reference: `docs/features/media/`

7. **ApiManagement**
   - ApiKeys, ApiRequests
   - Reference: `docs/features/api/`

8. **WebhookManagement**
   - Webhooks, WebhookDeliveries
   - Reference: `docs/features/api/`

### Priority 4: Security & Audit
9. **AuditManagement**
   - AuditLogs, AuditExports, RetentionPolicies
   - Reference: `docs/features/audit/`

10. **AuthenticationManagement**
    - UserSessions, LoginAttempts, MFA, PasswordResetRequests
    - Reference: `docs/features/authentication/`

### Priority 5: Workflow & System
11. **WorkflowManagement**
    - Workflows, WorkflowStages, WorkflowApprovals
    - Reference: `docs/features/workflow/`

12. **SystemManagement**
    - SystemMaintenance, SystemBackup, SystemError
    - Reference: `docs/features/system/`

---

## Common Commands by Feature

### Account Management
- CreateAccountCommand
- UpdateAccountCommand
- SuspendAccountCommand
- ReactivateAccountCommand
- DeleteAccountCommand
- ChangeSubscriptionCommand

### User Management
- CreateUserCommand
- ActivateUserCommand
- DeactivateUserCommand
- LockUserCommand
- UpdateUserCommand
- SendInvitationCommand

### Profile Management
- CreateProfileCommand
- UpdateProfileCommand
- DeleteProfileCommand
- ShareProfileCommand
- SetDefaultProfileCommand

---

## Testing the API

### Run the Application
```bash
cd src/StartupStarter.Api
dotnet run
```

### Test Endpoints

**Create Account**:
```bash
POST https://localhost:{port}/api/accounts
Content-Type: application/json

{
  "accountName": "Test Account",
  "accountType": 0,
  "ownerUserId": "user-123",
  "subscriptionTier": "Premium",
  "createdBy": "admin-456"
}
```

**Get Account**:
```bash
GET https://localhost:{port}/api/accounts/{accountId}
```

### Swagger UI
Access Swagger at: `https://localhost:{port}/swagger`

---

## Database Setup

### Create Database
```bash
cd src/StartupStarter.Api
dotnet ef database update
```

This will create the `StartupStarterDb` database in SQL Server LocalDB with all 32 tables.

---

## Build Status

✅ **Solution Build**: SUCCESS
✅ **All Projects**: Core, Infrastructure, API
✅ **EF Migrations**: Created
✅ **MediatR**: Configured
✅ **Account Management**: Fully Implemented

---

## Next Steps

1. Implement remaining features using the pattern above
2. Add validation to Commands
3. Add error handling middleware
4. Add authentication & authorization
5. Add logging
6. Add unit tests for Handlers
7. Add integration tests for Controllers

---

## Key Design Principles Followed

✅ No AutoMapper - using ToDto() extension methods
✅ No IRepository - using IStartupStarterContext directly
✅ Named IDs (AccountId, not Id)
✅ CQRS pattern with MediatR
✅ Feature-based folder structure
✅ One class per file
✅ Flattened namespaces

---

**Implementation Status**: Foundation Complete - Ready for Feature Development
