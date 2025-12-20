# User Management - Detailed Design

## Overview
User account lifecycle management including creation, activation, deactivation, and deletion.

## Aggregates
- **UserAggregate**: User accounts and profiles
- **UserInvitationAggregate**: User invitation workflow

## Key Features
- User creation and invitation
- Account activation/deactivation
- User locking (security)
- Account deletion (soft/hard)
- User profile management
- Role assignment
- Account transfer (rare)

## Dependencies
- **AccountAggregate**: Users belong to one account
- **RoleAggregate**: Users assigned roles
- **AuthenticationAggregate**: Login credentials
- **Email Service**: Invitation emails

## Business Rules

### User Creation
1. Users created via invitation
2. Email must be unique system-wide
3. User belongs to exactly one account
4. Initial roles assigned during creation
5. Invitation expires after 7 days
6. Password set on first login

### User Activation
1. Email verification required
2. Or admin can manually activate
3. Auto-activation for SSO users
4. Activation methods tracked

### User Deactivation
1. Deactivated users cannot login
2. Sessions terminated immediately
3. Reason required for deactivation
4. Can be reactivated by admin
5. Scheduled jobs not affected

### User Locking
1. Auto-lock after 5 failed login attempts
2. Lock duration: 15 minutes (or manual unlock)
3. Admin can lock/unlock manually
4. Reason required for manual lock
5. Lock expires automatically if duration set

### User Deletion
1. Soft delete by default
2. Hard delete requires admin approval
3. Cannot delete account owner (transfer first)
4. Deletion reason mandatory
5. 30-day recovery window for soft delete
6. Audit trail preserved

### Account Transfer
1. Extremely rare operation
2. Requires super admin approval
3. Reason mandatory
4. All user data moved
5. Roles reassessed for new account

## Data Model

**Users Table**
- UserId (PK), Email (unique)
- FirstName, LastName
- AccountId (FK, indexed)
- PasswordHash
- Status (Active, Inactive, Locked, Deleted)
- CreatedAt, UpdatedAt
- ActivatedAt, DeactivatedAt
- LockedAt, LockReason, LockDurationTicks
- DeletedAt, DeletionType, DeletionReason

**UserInvitations Table**
- InvitationId (PK), Email
- AccountId (FK), InvitedBy
- SentAt, ExpiresAt
- AcceptedAt, AcceptedByUserId
- IsAccepted, IsExpired
- RoleIds (JSON array)

## Sequence: Invite User
```
Admin → SendUserInvitationCommand
→ Validate admin has user:create permission
→ Check email not already in use
→ Create UserInvitation record
→ Generate invitation token
→ Save to database
→ Publish UserInvitationSentEvent
→ Send invitation email with link
→ Return invitation details
```

## Sequence: Accept Invitation
```
User clicks invitation link
→ AcceptUserInvitationCommand
→ Validate token not expired
→ Validate token not used
→ Create User record
→ Set initial password
→ Assign invited roles
→ Mark invitation as accepted
→ Publish UserCreatedEvent, InvitationAcceptedEvent
→ Send welcome email
→ Return user details
```

## Sequence: Lock User (Failed Logins)
```
Failed Login → LoginAttempt recorded
→ Check attempt count in last 15 min
→ If >= 5:
  → Lock user account
  → Set lock duration: 15 minutes
  → Publish UserLockedEvent
  → Terminate all sessions
  → Send security notification email
```

## API Endpoints
- POST /api/users/invite - Invite user
- POST /api/users/accept-invitation - Accept invitation
- GET /api/users - List users (paginated)
- GET /api/users/{id} - Get user
- PUT /api/users/{id} - Update user
- DELETE /api/users/{id} - Delete user
- POST /api/users/{id}/activate - Activate user
- POST /api/users/{id}/deactivate - Deactivate user
- POST /api/users/{id}/lock - Lock user
- POST /api/users/{id}/unlock - Unlock user
- PUT /api/users/{id}/account - Transfer account (super admin)
- GET /api/users/{id}/roles - Get user roles
- GET /api/invitations - List pending invitations
- DELETE /api/invitations/{id} - Cancel invitation

## User Status State Machine
```
[Invited] → Accept → [Active]
          → Expire → [Expired]

[Active] → Deactivate → [Inactive]
         → Lock → [Locked]
         → Delete → [Deleted]

[Inactive] → Activate → [Active]
           → Delete → [Deleted]

[Locked] → Unlock → [Active]
         → Auto-expire (15 min) → [Active]

[Deleted (Soft)] → Restore (within 30 days) → [Inactive]
                 → Hard Delete (after 30 days) → [Permanently Deleted]
```

## Security
- Passwords hashed with Argon2id
- Failed login attempt tracking
- Automatic account locking
- Session termination on status change
- Email verification for sensitive changes
- Admin actions require MFA
- Audit all user modifications

## Performance
- User lookups cached (5 min TTL)
- Indexed on Email, AccountId
- Pagination for user lists
- Async invitation emails
- Batch operations support

## Email Templates
1. **Invitation Email**: Link to accept invitation
2. **Welcome Email**: Account created successfully
3. **Account Locked**: Security notification
4. **Account Deactivated**: Notification with reason
5. **Account Deletion**: Confirmation and recovery info
