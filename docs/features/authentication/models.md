# Authentication Models

## Core Aggregate

### AuthenticationAggregate

Located in: `StartupStarter.Core\Model\AuthenticationAggregate\`

#### Entities

**UserSession.cs** (Aggregate Root)
```csharp
public class UserSession
{
    public string SessionId { get; private set; }
    public string UserId { get; private set; }
    public string AccountId { get; private set; }
    public string IPAddress { get; private set; }
    public string UserAgent { get; private set; }
    public LoginMethod LoginMethod { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? LoggedOutAt { get; private set; }
    public LogoutType? LogoutType { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void EndSession(LogoutType logoutType);
}
```

**LoginAttempt.cs**
```csharp
public class LoginAttempt
{
    public string LoginAttemptId { get; private set; }
    public string UserId { get; private set; }
    public string Email { get; private set; }
    public string IPAddress { get; private set; }
    public string UserAgent { get; private set; }
    public LoginMethod LoginMethod { get; private set; }
    public bool Success { get; private set; }
    public FailureReason? FailureReason { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime Timestamp { get; private set; }
}
```

**MultiFactorAuthentication.cs**
```csharp
public class MultiFactorAuthentication
{
    public string MfaId { get; private set; }
    public string UserId { get; private set; }
    public string AccountId { get; private set; }
    public MfaMethod Method { get; private set; }
    public bool IsEnabled { get; private set; }
    public string EnabledBy { get; private set; }
    public DateTime EnabledAt { get; private set; }
    public DateTime? DisabledAt { get; private set; }
    public string DisabledBy { get; private set; }
    public string DisabledReason { get; private set; }
    public string SecretKey { get; private set; }
    public string BackupCodesJson { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Enable(MfaMethod method, string enabledBy);
    public void Disable(string disabledBy, string reason);
}
```

**PasswordResetRequest.cs**
```csharp
public class PasswordResetRequest
{
    public string ResetRequestId { get; private set; }
    public string UserId { get; private set; }
    public string Email { get; private set; }
    public string IPAddress { get; private set; }
    public string ResetTokenHash { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public ResetMethod? ResetMethod { get; private set; }
    public bool IsCompleted { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Complete(ResetMethod method);
}
```

#### Enums

**LoginMethod.cs**
```csharp
public enum LoginMethod
{
    Password,
    SSO,
    OAuth,
    MFA
}
```

**LogoutType.cs**
```csharp
public enum LogoutType
{
    Manual,
    SessionExpired,
    ForcedLogout
}
```

**FailureReason.cs**
```csharp
public enum FailureReason
{
    InvalidCredentials,
    AccountLocked,
    AccountDisabled,
    MFAFailed
}
```

**MfaMethod.cs**
```csharp
public enum MfaMethod
{
    SMS,
    Email,
    AuthenticatorApp,
    HardwareToken
}
```

**ResetMethod.cs**
```csharp
public enum ResetMethod
{
    Email,
    AdminReset,
    SecurityQuestions
}
```

#### Domain Events

**UserLoginAttemptedEvent.cs**
```csharp
public class UserLoginAttemptedEvent : DomainEvent
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public LoginMethod LoginMethod { get; set; }
}
```

**UserLoginSucceededEvent.cs**
```csharp
public class UserLoginSucceededEvent : DomainEvent
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string AccountId { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public LoginMethod LoginMethod { get; set; }
    public string SessionId { get; set; }
}
```

**UserLoginFailedEvent.cs**
```csharp
public class UserLoginFailedEvent : DomainEvent
{
    public string Email { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public FailureReason FailureReason { get; set; }
    public int AttemptCount { get; set; }
}
```

**UserLogoutInitiatedEvent.cs**
```csharp
public class UserLogoutInitiatedEvent : DomainEvent
{
    public string UserId { get; set; }
    public string AccountId { get; set; }
    public string SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    public LogoutType LogoutType { get; set; }
}
```

**UserSessionExpiredEvent.cs**
```csharp
public class UserSessionExpiredEvent : DomainEvent
{
    public string UserId { get; set; }
    public string AccountId { get; set; }
    public string SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan SessionDuration { get; set; }
}
```

**UserMfaEnabledEvent.cs**
```csharp
public class UserMfaEnabledEvent : DomainEvent
{
    public string UserId { get; set; }
    public string AccountId { get; set; }
    public MfaMethod MfaMethod { get; set; }
    public string EnabledBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**UserMfaDisabledEvent.cs**
```csharp
public class UserMfaDisabledEvent : DomainEvent
{
    public string UserId { get; set; }
    public string AccountId { get; set; }
    public string DisabledBy { get; set; }
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; }
}
```

**UserPasswordResetRequestedEvent.cs**
```csharp
public class UserPasswordResetRequestedEvent : DomainEvent
{
    public string Email { get; set; }
    public string IPAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string ResetToken { get; set; }
}
```

**UserPasswordResetCompletedEvent.cs**
```csharp
public class UserPasswordResetCompletedEvent : DomainEvent
{
    public string UserId { get; set; }
    public string AccountId { get; set; }
    public DateTime Timestamp { get; set; }
    public ResetMethod ResetMethod { get; set; }
}
```

## Infrastructure

### Entity Configuration

**UserSessionConfiguration.cs**
```csharp
public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");
        builder.HasKey(s => s.SessionId);

        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.AccountId).IsRequired();
        builder.Property(s => s.IPAddress).HasMaxLength(45);
        builder.Property(s => s.UserAgent).HasMaxLength(500);

        builder.HasIndex(s => new { s.UserId, s.IsActive });

        builder.Ignore(s => s.DomainEvents);
    }
}
```

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\Authentication\Commands\`

**LoginCommand.cs**
```csharp
public class LoginCommand : IRequest<SessionDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
}
```

**LogoutCommand.cs**
```csharp
public class LogoutCommand : IRequest<bool>
{
    public string SessionId { get; set; }
    public string UserId { get; set; }
}
```

**EnableMfaCommand.cs**
```csharp
public class EnableMfaCommand : IRequest<bool>
{
    public string UserId { get; set; }
    public MfaMethod Method { get; set; }
    public string EnabledBy { get; set; }
}
```

**RequestPasswordResetCommand.cs**
```csharp
public class RequestPasswordResetCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string IPAddress { get; set; }
}
```
