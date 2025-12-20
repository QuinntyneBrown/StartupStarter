# Account Management - Detailed Design

## Overview

The Account Management feature handles the creation, management, and lifecycle of tenant accounts in the system. Each account represents an organization/tenant that can have multiple users, profiles, and content.

## Architecture

### Bounded Context

Account Management is a core bounded context in the system with the following responsibilities:
- Account lifecycle management (create, update, suspend, delete)
- Subscription tier management
- Account ownership transfer
- Account-level settings management
- Profile association tracking

### Domain Model

See [domain-model.puml](domain-model.puml) for the complete domain model diagram.

**Key Aggregates:**
- **AccountAggregate**: Root aggregate containing Account and AccountSettings entities

**Key Entities:**
- **Account**: Aggregate root representing a tenant/organization
- **AccountSettings**: Configuration settings for an account

**Key Enums:**
- **AccountType**: Individual, Team, Enterprise
- **AccountStatus**: Active, Suspended, Deleted
- **DeletionType**: SoftDelete, HardDelete

## Dependencies

### Internal Dependencies
- **UserAggregate**: Account references OwnerUserId
- **ProfileAggregate**: Accounts can have multiple profiles
- **AuditAggregate**: All account operations are audited

### External Dependencies
- **Billing Service**: For subscription tier changes
- **Email Service**: For account notifications
- **Event Bus**: For publishing domain events

## Constraints

### Business Rules

1. **Account Creation**
   - Account name must be unique within the system
   - Owner user must exist in the system
   - Initial subscription tier must be valid
   - Account type cannot be changed after creation

2. **Account Suspension**
   - Only Active accounts can be suspended
   - Must provide a suspension reason
   - Optional suspension duration (permanent if not specified)
   - All users in the account are affected

3. **Account Deletion**
   - Can be soft or hard delete
   - Must track counts of affected users, profiles, and content
   - Hard delete requires additional confirmation
   - Cannot delete if active subscriptions exist (business rule)

4. **Ownership Transfer**
   - New owner must be an existing user in the account
   - Previous owner loses owner privileges
   - Can only be performed by current owner or system admin

5. **Subscription Changes**
   - Downgrades may require approval
   - Effective date can be immediate or scheduled
   - Must validate tier exists and is available

### Technical Constraints

1. **Performance**
   - Account lookup by ID must be < 100ms
   - Account creation must be < 500ms
   - Batch operations limited to 100 accounts

2. **Data Integrity**
   - Account ID is immutable
   - Soft deletes maintain data for compliance
   - Audit trail required for all changes

3. **Concurrency**
   - Optimistic concurrency control using EF Core row versioning
   - Version token included in all update operations

## Sequence Diagrams

### Create Account Flow
See [sequence-create-account.puml](sequence-create-account.puml)

**Steps:**
1. User/Admin sends CreateAccountCommand
2. Handler validates command
3. Account aggregate root is created
4. Business rules are validated
5. AccountCreatedEvent is raised
6. Changes are persisted to database
7. Domain events are published
8. AccountDto is returned

### Suspend Account Flow

```
User → API → Handler → Account.Suspend()
Account validates can be suspended
Account raises AccountSuspendedEvent
Handler saves to DB
Handler publishes events
Users in account are notified
```

### Change Subscription Tier Flow

```
User → API → Handler → Account.ChangeSubscriptionTier()
Account validates tier exists
Account raises AccountSubscriptionChangedEvent
Handler saves to DB
Handler publishes events
Billing system is notified
```

## C4 Diagrams

### Context Diagram

```
┌─────────────────────────────────────────────────────────┐
│                    StartupStarter System                 │
│                                                          │
│  ┌──────────────────────────────────────────────────┐   │
│  │         Account Management Context               │   │
│  │                                                  │   │
│  │  - Manages tenant accounts                      │   │
│  │  - Subscription management                      │   │
│  │  - Account lifecycle                            │   │
│  └──────────────────────────────────────────────────┘   │
│                          │                               │
│                          ▼                               │
│  ┌──────────────────────────────────────────────────┐   │
│  │           User Management Context                │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
         │                                    │
         ▼                                    ▼
   [Admin User]                         [Billing System]
```

### Container Diagram

```
┌─────────────────────────────────────────────────┐
│          Account Management API                  │
│                                                  │
│  ┌────────────────────────────────────────────┐ │
│  │  Controllers                               │ │
│  │  - AccountsController                      │ │
│  └────────────────────────────────────────────┘ │
│                     │                            │
│                     ▼                            │
│  ┌────────────────────────────────────────────┐ │
│  │  Application Layer (MediatR)               │ │
│  │  - Commands: Create, Update, Delete, etc.  │ │
│  │  - Queries: GetById, GetByOwner, etc.      │ │
│  └────────────────────────────────────────────┘ │
│                     │                            │
│                     ▼                            │
│  ┌────────────────────────────────────────────┐ │
│  │  Domain Layer                              │ │
│  │  - AccountAggregate                        │ │
│  │  - Business Rules                          │ │
│  │  - Domain Events                           │ │
│  └────────────────────────────────────────────┘ │
│                     │                            │
│                     ▼                            │
│  ┌────────────────────────────────────────────┐ │
│  │  Infrastructure Layer                      │ │
│  │  - StartupStarterContext (EF Core)         │ │
│  │  - Entity Configurations                   │ │
│  └────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
                      │
                      ▼
              [SQL Server Database]
```

## Data Model

### Database Tables

**Accounts Table**
```sql
CREATE TABLE Accounts (
    AccountId NVARCHAR(100) PRIMARY KEY,
    AccountName NVARCHAR(200) NOT NULL,
    AccountType INT NOT NULL,
    OwnerUserId NVARCHAR(100) NOT NULL,
    SubscriptionTier NVARCHAR(50) NOT NULL,
    Status INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    DeletedAt DATETIME2 NULL,
    SuspendedAt DATETIME2 NULL,
    SuspensionReason NVARCHAR(500) NULL,
    SuspensionDurationTicks BIGINT NULL,
    RowVersion ROWVERSION,
    CONSTRAINT FK_Accounts_Users FOREIGN KEY (OwnerUserId)
        REFERENCES Users(UserId)
);

CREATE UNIQUE INDEX IX_Accounts_AccountName
    ON Accounts(AccountName)
    WHERE DeletedAt IS NULL;

CREATE INDEX IX_Accounts_OwnerUserId
    ON Accounts(OwnerUserId);

CREATE INDEX IX_Accounts_Status
    ON Accounts(Status);
```

**AccountSettings Table**
```sql
CREATE TABLE AccountSettings (
    AccountSettingsId NVARCHAR(100) PRIMARY KEY,
    AccountId NVARCHAR(100) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    SettingsJson NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_AccountSettings_Accounts FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId) ON DELETE CASCADE
);

CREATE INDEX IX_AccountSettings_AccountId_Category
    ON AccountSettings(AccountId, Category);
```

## API Endpoints

### REST API

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | /api/accounts | Create new account | Admin |
| GET | /api/accounts/{id} | Get account by ID | User |
| GET | /api/accounts | Get all accounts (paginated) | Admin |
| GET | /api/accounts/owner/{userId} | Get accounts by owner | User |
| PUT | /api/accounts/{id} | Update account | Owner/Admin |
| DELETE | /api/accounts/{id} | Delete account | Admin |
| POST | /api/accounts/{id}/suspend | Suspend account | Admin |
| POST | /api/accounts/{id}/reactivate | Reactivate account | Admin |
| PUT | /api/accounts/{id}/subscription | Change subscription | Owner/Admin |
| PUT | /api/accounts/{id}/owner | Transfer ownership | Owner/Admin |
| PUT | /api/accounts/{id}/settings | Update settings | Owner/Admin |

## Event Handlers

### Domain Events Published

1. **AccountCreatedEvent**
   - Triggers: Welcome email, initial setup, audit log
   - Consumers: Email Service, Audit Service, Analytics Service

2. **AccountSuspendedEvent**
   - Triggers: User notifications, access revocation
   - Consumers: Authentication Service, Email Service

3. **AccountDeletedEvent**
   - Triggers: Cleanup tasks, data archival
   - Consumers: Storage Service, Audit Service

4. **AccountSubscriptionChangedEvent**
   - Triggers: Billing updates, feature access changes
   - Consumers: Billing Service, Feature Toggle Service

## Testing Strategy

### Unit Tests
- Account aggregate business logic
- Domain event raising
- Validation rules
- Command handlers

### Integration Tests
- Database persistence
- API endpoints
- Event publishing
- Cross-aggregate operations

### Test Scenarios
1. Create account with valid data
2. Create account with duplicate name (should fail)
3. Suspend active account
4. Suspend already suspended account (should fail)
5. Transfer ownership to non-existent user (should fail)
6. Change subscription tier
7. Delete account (soft and hard)
8. Update account settings

## Security Considerations

1. **Authorization**
   - Only account owners and admins can modify accounts
   - Users can only view accounts they belong to
   - Admins have full access

2. **Data Protection**
   - Sensitive account data encrypted at rest
   - PII handled according to GDPR requirements
   - Soft deletes for compliance

3. **Audit Trail**
   - All account modifications logged
   - IP address and user tracked
   - Before/after states captured

## Performance Optimization

1. **Caching Strategy**
   - Account lookups cached (15 min TTL)
   - Cache invalidated on updates
   - Distributed cache for multi-instance deployments

2. **Query Optimization**
   - Indexes on frequently queried fields
   - Pagination for list queries
   - Projection to DTOs to minimize data transfer

3. **Event Processing**
   - Asynchronous event handlers
   - Retry logic for failed events
   - Dead letter queue for persistent failures

## Migration Path

For existing data:
1. Create AccountAggregate structure
2. Migrate existing account data
3. Populate AccountSettings from legacy config
4. Update foreign keys
5. Deploy with feature flag
6. Gradually enable for tenants
7. Monitor and adjust
