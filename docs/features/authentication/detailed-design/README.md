# Authentication - Detailed Design

## Overview

Authentication manages user login, sessions, MFA, and password reset functionality.

## Key Components

### Aggregates

**UserSession**: Active user sessions
**LoginAttempt**: Login attempt tracking
**MultiFactorAuthentication**: MFA configuration per user
**PasswordResetRequest**: Password reset token management

## Dependencies

- **UserAggregate**: Authentication validates against users
- **AccountAggregate**: Session tied to account
- **AuditAggregate**: All auth events audited

## Constraints

### Business Rules

1. **Login**
   - Maximum 5 failed attempts before account lock (15 min)
   - Session timeout: 24 hours (configurable)
   - Concurrent sessions allowed (max 5 per user)

2. **MFA**
   - Required for admin users
   - Optional for regular users
   - Backup codes generated (10 codes)
   - TOTP algorithm (30-second window)

3. **Password Reset**
   - Reset tokens valid for 1 hour
   - One-time use only
   - Must not match last 5 passwords

### Technical Constraints

- Password hashing: Argon2id
- Session tokens: JWT with RS256
- Token refresh: Refresh token rotation
- MFA: TOTP (RFC 6238)

## Sequence Diagrams

### Login Flow with MFA
```
User → API: POST /auth/login
→ Validate credentials
→ Check MFA enabled
→ Send MFA code (if enabled)
User → API: POST /auth/verify-mfa
→ Validate MFA code
→ Create session
→ Generate JWT tokens
→ Record login event
→ Return tokens to user
```

### Password Reset Flow
```
User → API: POST /auth/forgot-password
→ Validate email exists
→ Generate reset token
→ Send email with token link
User clicks link in email
User → API: POST /auth/reset-password
→ Validate token not expired
→ Validate token not used
→ Hash new password
→ Invalidate token
→ Record password reset event
```

## Data Model

### UserSessions Table
- SessionId (PK)
- UserId (FK, indexed)
- AccountId (FK)
- IPAddress
- UserAgent
- LoginMethod
- CreatedAt
- ExpiresAt
- IsActive (indexed)

### MultiFactorAuthentications Table
- MfaId (PK)
- UserId (FK, unique)
- Method (SMS, Email, AuthenticatorApp)
- SecretKey (encrypted)
- BackupCodesJson (encrypted)
- IsEnabled

### PasswordResetRequests Table
- ResetRequestId (PK)
- UserId (FK)
- Email
- ResetTokenHash
- RequestedAt
- ExpiresAt
- IsCompleted

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | User login |
| POST | /api/auth/verify-mfa | Verify MFA code |
| POST | /api/auth/logout | Logout |
| POST | /api/auth/refresh | Refresh access token |
| POST | /api/auth/forgot-password | Request password reset |
| POST | /api/auth/reset-password | Complete password reset |
| POST | /api/auth/enable-mfa | Enable MFA |
| POST | /api/auth/disable-mfa | Disable MFA |
| GET | /api/auth/sessions | List active sessions |
| DELETE | /api/auth/sessions/{id} | Revoke session |

## Security

- All endpoints HTTPS only
- Rate limiting on auth endpoints
- JWT tokens include:
  - UserId, AccountId, Roles
  - Expiration time
  - Token ID for revocation
- Password requirements:
  - Minimum 8 characters
  - At least 1 uppercase, 1 lowercase, 1 digit, 1 special char
- Failed login attempt tracking
- IP-based anomaly detection
