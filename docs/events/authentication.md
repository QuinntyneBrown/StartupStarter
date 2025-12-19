# Authentication Events

## User.Login.Attempted
**Description**: Fired when a user attempts to log into the system

**Payload**:
- UserId: string (optional, if recognized)
- Email: string
- IPAddress: string
- UserAgent: string
- Timestamp: DateTime
- LoginMethod: enum (Password, SSO, OAuth, MFA)

---

## User.Login.Succeeded
**Description**: Fired when a user successfully logs into the system

**Payload**:
- UserId: string
- Email: string
- AccountId: string
- IPAddress: string
- UserAgent: string
- Timestamp: DateTime
- LoginMethod: enum (Password, SSO, OAuth, MFA)
- SessionId: string

---

## User.Login.Failed
**Description**: Fired when a user login attempt fails

**Payload**:
- Email: string
- IPAddress: string
- UserAgent: string
- Timestamp: DateTime
- FailureReason: enum (InvalidCredentials, AccountLocked, AccountDisabled, MFAFailed)
- AttemptCount: int

---

## User.Logout.Initiated
**Description**: Fired when a user logs out of the system

**Payload**:
- UserId: string
- AccountId: string
- SessionId: string
- Timestamp: DateTime
- LogoutType: enum (Manual, SessionExpired, ForcedLogout)

---

## User.Session.Expired
**Description**: Fired when a user session expires

**Payload**:
- UserId: string
- AccountId: string
- SessionId: string
- Timestamp: DateTime
- SessionDuration: TimeSpan

---

## User.MFA.Enabled
**Description**: Fired when multi-factor authentication is enabled for a user

**Payload**:
- UserId: string
- AccountId: string
- MFAMethod: enum (SMS, Email, AuthenticatorApp, HardwareToken)
- EnabledBy: string (UserId or AdminId)
- Timestamp: DateTime

---

## User.MFA.Disabled
**Description**: Fired when multi-factor authentication is disabled for a user

**Payload**:
- UserId: string
- AccountId: string
- DisabledBy: string (UserId or AdminId)
- Timestamp: DateTime
- Reason: string

---

## User.Password.ResetRequested
**Description**: Fired when a password reset is requested

**Payload**:
- Email: string
- IPAddress: string
- Timestamp: DateTime
- ResetToken: string (hashed)

---

## User.Password.ResetCompleted
**Description**: Fired when a password reset is completed

**Payload**:
- UserId: string
- AccountId: string
- Timestamp: DateTime
- ResetMethod: enum (Email, AdminReset, SecurityQuestions)
