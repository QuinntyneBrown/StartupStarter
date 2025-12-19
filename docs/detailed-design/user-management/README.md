# User Management - Detailed Design

## Overview

The User Management module is responsible for managing user accounts, user invitations, user lifecycle (activation, deactivation), and security features (locking, unlocking). It follows Clean Architecture principles, Domain-Driven Design (DDD), and implements the CQRS pattern using MediatR.

## Architecture

This module is built following Clean Architecture layers:

- **Presentation Layer**: API Controllers exposing RESTful endpoints
- **Application Layer**: MediatR Commands, Queries, Handlers, and DTOs
- **Domain Layer**: Aggregates (User, UserInvitation), Domain Events, and Business Logic
- **Infrastructure Layer**: Repositories, External Services, and Event Dispatchers

## Key Components

### Domain Model

The domain model consists of:

1. **User Aggregate Root**
   - Central entity managing user identity and state
   - Enforces business invariants
   - Raises domain events for state changes
   - Associated with a single Account

2. **UserInvitation Entity**
   - Manages invitation lifecycle
   - Tracks invitation status (Pending, Accepted, Expired)
   - Links to created user upon acceptance

3. **Supporting Entities**
   - UserRole: Tracks role assignments
   - InvitationRole: Defines roles for invited users

### State Management

#### User Status Flow

```
Pending → Active → Inactive
    ↓        ↓        ↓
    └────> Suspended ←┘
```

- **Pending**: User created but not yet activated
- **Active**: User can access the system
- **Inactive**: User deactivated (can be reactivated)
- **Suspended**: User temporarily suspended (administrative action)

#### Invitation Status Flow

```
Pending → Accepted
    ↓
Expired/Cancelled
```

## User Lifecycle Workflows

### 1. Direct User Creation

**Scenario**: Admin directly creates a user account

**Steps**:
1. Admin initiates user creation via API
2. System validates email uniqueness and account existence
3. User aggregate is created with `Pending` status
4. `UserCreatedDomainEvent` is raised
5. Optional welcome email is sent
6. Admin can manually activate the user

**Business Rules**:
- Email must be unique within the system
- Account must exist
- At least one role must be assigned
- User starts in `Pending` status

### 2. Invitation-Based User Creation

**Scenario**: User is invited and accepts invitation

#### Phase 1: Send Invitation

**Steps**:
1. Admin/User sends invitation via API
2. System validates email not already invited or registered
3. UserInvitation entity is created
4. Invitation roles are assigned
5. `UserInvitationSentDomainEvent` is raised
6. Invitation email with unique link is sent
7. Invitation expires after 7 days (configurable)

**Business Rules**:
- Email must not have pending invitation
- Email must not be registered
- Invitation expires after configured duration
- Inviter must have permission to invite

#### Phase 2: Accept Invitation

**Steps**:
1. Invited user clicks invitation link
2. User provides first name, last name, and password
3. System validates invitation is valid and not expired
4. User aggregate is created with roles from invitation
5. Invitation status updated to `Accepted`
6. `UserInvitationAcceptedDomainEvent` is raised
7. `UserCreatedDomainEvent` is raised
8. User is auto-activated via `EmailVerification` method
9. `UserActivatedDomainEvent` is raised
10. Welcome email is sent

**Business Rules**:
- Invitation must exist and be in `Pending` status
- Invitation must not be expired
- Email from invitation is used for user account
- User is automatically activated upon acceptance
- Invitation can only be accepted once

### 3. User Activation

**Activation Methods**:

1. **EmailVerification**: User activates via email link
2. **AdminActivation**: Admin manually activates user
3. **AutoActivation**: System automatically activates (e.g., after invitation acceptance)

**Steps**:
1. Activation request received (via email link or admin action)
2. System validates user exists and is not already active
3. User status changed to `Active`
4. `UserActivatedDomainEvent` is raised
5. Activation notification sent to user

**Business Rules**:
- User must be in `Pending` or `Inactive` status
- Cannot activate a locked user without unlocking first
- Cannot activate a deleted user

### 4. User Deactivation

**Scenario**: Admin deactivates a user account

**Steps**:
1. Admin initiates deactivation with reason
2. System validates user exists and is currently active
3. User status changed to `Inactive`
4. `UserDeactivatedDomainEvent` is raised
5. All active user sessions are revoked
6. Deactivation notification sent to user

**Business Rules**:
- User must be in `Active` status
- Reason must be provided
- Only admins can deactivate users
- User cannot deactivate themselves

### 5. User Deletion

**Deletion Types**:

1. **SoftDelete**: User marked as deleted but data retained
2. **HardDelete**: User permanently removed from database (rarely used)

**Steps**:
1. Admin initiates deletion with reason and type
2. System validates user exists
3. User marked as deleted
4. `UserDeletedDomainEvent` is raised
5. All active sessions terminated
6. Associated data handled per deletion type
7. Deletion notification sent if soft delete

**Business Rules**:
- Only admins can delete users
- Deletion reason required
- Soft delete is default
- Hard delete requires special permission
- Cannot delete the last admin in an account

## Invitation System

### Invitation Creation

**Features**:
- Admin or authorized user can send invitations
- Multiple roles can be assigned to invitation
- Customizable expiration period (default: 7 days)
- Invitation link contains secure token
- Email sent with acceptance instructions

**Validation**:
- Email must not be already registered
- Email must not have pending invitation
- Inviter must have permission
- Account must be active

### Invitation Expiration

**Automated Process**:
- Background job runs periodically
- Identifies invitations past expiration date
- Updates status to `Expired`
- Raises `UserInvitationExpiredDomainEvent`
- Optionally notifies inviter

**Manual Expiration**:
- Admin can manually cancel pending invitations
- Invitation status set to `Cancelled`

### Invitation Acceptance

**Process**:
1. User receives email with invitation link
2. Link contains invitation ID and secure token
3. User provides required information
4. System creates user account
5. Assigns roles from invitation
6. Auto-activates user
7. Marks invitation as accepted

**Security**:
- Token validated server-side
- One-time use link
- Expiration enforced
- Secure password requirements

## Security Considerations

### User Locking

**Automatic Locking** (System-Initiated):

**Triggers**:
- Failed login attempts exceed threshold (e.g., 5 attempts)
- Suspicious activity detected
- Security policy violation

**Process**:
1. Security system detects trigger event
2. User account automatically locked
3. Lock duration set (e.g., 30 minutes)
4. `UserLockedDomainEvent` raised with `LockedBy: "System"`
5. Security alert email sent to user
6. Admin notification of security event
7. Lock automatically expires after duration

**Features**:
- Temporary lock with auto-expiration
- Prevents brute force attacks
- Audit trail of lock events

**Manual Locking** (Admin-Initiated):

**Scenarios**:
- Security investigation
- Policy violation
- Account compromise suspected
- User request

**Process**:
1. Admin initiates lock with reason
2. User account locked indefinitely
3. `UserLockedDomainEvent` raised with admin ID
4. All active sessions terminated
5. Lock notification sent to user
6. Requires manual unlock by admin

**Features**:
- No automatic expiration
- Requires admin intervention to unlock
- Detailed reason tracking

### User Unlocking

**Process**:
1. Admin reviews lock reason
2. Initiates unlock action
3. Lock status cleared
4. `UserUnlockedDomainEvent` raised
5. Unlock notification sent to user
6. User can log in again

**Business Rules**:
- Only admins can unlock users
- Cannot unlock a deactivated user
- Unlock reason should be documented
- Failed login counter reset

### Account Security Features

1. **Failed Login Tracking**
   - Track failed login attempts
   - Increment counter on failure
   - Reset counter on successful login
   - Trigger auto-lock at threshold

2. **Session Management**
   - Track active user sessions
   - Terminate sessions on deactivation
   - Terminate sessions on lock
   - Support multi-device sessions

3. **Password Management**
   - Enforce password complexity
   - Support password reset
   - Track password history
   - Enforce password expiration

4. **Audit Trail**
   - All user actions logged
   - Domain events provide audit trail
   - Track who/when/what for all changes
   - Immutable event history

## Domain Events

All state changes raise domain events for:

- **Audit Trail**: Complete history of user changes
- **Event Sourcing**: Reconstruct state from events
- **Integration**: Notify other bounded contexts
- **Side Effects**: Trigger emails, notifications, workflows

### Event Processing

Events are processed after database transaction commits:

1. **Domain Events Raised**: During aggregate method calls
2. **Transaction Commits**: Changes persisted to database
3. **Events Dispatched**: After successful commit
4. **Handlers Execute**: Process events asynchronously
5. **Side Effects**: Send emails, publish to message bus

### Event Handlers

Common event handler patterns:

- **Email Notifications**: Send emails on user events
- **Audit Logging**: Log to audit trail
- **Integration Events**: Publish to message bus for other services
- **Workflow Triggers**: Start background workflows

## CQRS Implementation

### Commands (Write Operations)

Commands modify state and raise domain events:

- CreateUserCommand
- UpdateUserCommand
- ActivateUserCommand
- DeactivateUserCommand
- LockUserCommand
- UnlockUserCommand
- DeleteUserCommand
- SendUserInvitationCommand
- AcceptUserInvitationCommand

### Queries (Read Operations)

Queries retrieve data without side effects:

- GetUserByIdQuery
- GetUserByEmailQuery
- GetUsersByAccountQuery
- GetUserInvitationByIdQuery
- GetPendingInvitationsByAccountQuery

### Benefits

- **Separation of Concerns**: Read and write models separated
- **Scalability**: Read and write can scale independently
- **Optimization**: Queries optimized for display, commands for business logic
- **Security**: Different validation and authorization for reads/writes

## Repository Pattern

### IUserRepository

Provides data access abstraction for User aggregate:

```csharp
Task<User?> GetByIdAsync(Guid userId);
Task<User?> GetByEmailAsync(string email);
Task<PaginatedList<User>> GetPagedByAccountIdAsync(...);
Task AddAsync(User user);
Task UpdateAsync(User user);
Task SaveChangesAsync();
```

### IUserInvitationRepository

Provides data access for UserInvitation entity:

```csharp
Task<UserInvitation?> GetByIdAsync(Guid invitationId);
Task<UserInvitation?> GetByEmailAsync(string email, Guid accountId);
Task<List<UserInvitation>> GetExpiredInvitationsAsync();
Task AddAsync(UserInvitation invitation);
Task SaveChangesAsync();
```

## Validation

### Command Validation

FluentValidation used for command validation:

- Email format validation
- Required field validation
- Business rule validation
- Cross-field validation

**Validation Pipeline**:
1. Command sent via MediatR
2. Validation behavior intercepts
3. FluentValidation rules executed
4. Validation errors returned if invalid
5. Handler executed if valid

### Domain Validation

Business rules enforced in domain entities:

- User cannot have duplicate email
- User must belong to an account
- Invitation cannot be accepted twice
- User cannot be activated if deleted

## Error Handling

### Common Error Scenarios

1. **User Not Found**: Return 404 Not Found
2. **Email Already Exists**: Return 409 Conflict
3. **Invitation Expired**: Return 400 Bad Request
4. **Unauthorized Action**: Return 403 Forbidden
5. **Validation Failure**: Return 400 Bad Request with details

### Exception Types

- **DomainException**: Business rule violation
- **NotFoundException**: Entity not found
- **ValidationException**: Validation failed
- **UnauthorizedException**: Permission denied

## Integration Points

### External Services

1. **Email Service**
   - Welcome emails
   - Invitation emails
   - Activation notifications
   - Security alerts

2. **Notification Service**
   - Real-time notifications
   - Status change alerts
   - Security events

3. **Identity Service**
   - Authentication
   - Authorization
   - Current user context

4. **Message Queue**
   - Publish integration events
   - Async processing
   - Inter-service communication

### Related Bounded Contexts

- **Account Management**: User belongs to Account
- **Role Management**: User assigned roles
- **Authentication**: User credentials and sessions
- **Audit Trail**: All user events logged

## API Endpoints

### User Management

```
GET    /api/users                  - List users (paginated)
GET    /api/users/{id}             - Get user by ID
GET    /api/users/email/{email}    - Get user by email
POST   /api/users                  - Create user
PUT    /api/users/{id}             - Update user
DELETE /api/users/{id}             - Delete user
POST   /api/users/{id}/activate    - Activate user
POST   /api/users/{id}/deactivate  - Deactivate user
POST   /api/users/{id}/lock        - Lock user
POST   /api/users/{id}/unlock      - Unlock user
```

### Invitation Management

```
GET    /api/invitations                  - List invitations
GET    /api/invitations/{id}             - Get invitation by ID
POST   /api/invitations                  - Send invitation
POST   /api/invitations/{id}/accept      - Accept invitation
DELETE /api/invitations/{id}             - Cancel invitation
```

## Diagrams

This folder contains the following PlantUML diagrams:

1. **domain-model.puml**
   - Domain entities and relationships
   - Aggregate boundaries
   - Domain events
   - Enumerations

2. **sequence-diagrams.puml**
   - Create User Flow
   - Send Invitation Flow
   - Accept Invitation Flow
   - Activate/Deactivate User Flow
   - Lock/Unlock User Flow

3. **component-diagram.puml**
   - Clean Architecture layers
   - Component dependencies
   - CQRS implementation
   - Integration points

## Testing Considerations

### Unit Tests

- Domain entity business logic
- Command/Query handlers
- Validators
- Domain event raising

### Integration Tests

- API endpoint tests
- Database integration
- Email service integration
- Event dispatcher integration

### Test Scenarios

1. **Happy Path**: Standard user creation and activation
2. **Invitation Flow**: Complete invitation acceptance
3. **Security**: Lock/unlock scenarios
4. **Edge Cases**: Expired invitations, duplicate emails
5. **Error Cases**: Invalid data, unauthorized access

## Performance Considerations

### Optimization Strategies

1. **Pagination**: All list endpoints support pagination
2. **Caching**: Cache user data for frequent reads
3. **Indexes**: Database indexes on email, accountId
4. **Async Processing**: Event handlers run asynchronously
5. **Bulk Operations**: Support bulk user imports

### Scalability

- Read replicas for query operations
- Message queue for event processing
- Horizontal scaling of API layer
- Database sharding by account

## Future Enhancements

1. **Multi-Factor Authentication**: Add MFA support
2. **Social Login**: Support OAuth providers
3. **User Groups**: Organize users into groups
4. **Advanced Permissions**: Fine-grained permission system
5. **User Analytics**: Track user activity and engagement
6. **Bulk Operations**: Bulk user import/export
7. **Custom Fields**: Support custom user attributes
8. **Workflow Automation**: Automated user lifecycle workflows
