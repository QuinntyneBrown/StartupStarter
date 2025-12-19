# Authentication Domain Models

This document defines the C# models needed to implement the authentication events following Clean Architecture principles with MediatR.

## Domain Entities

### UserSession (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class UserSession : AggregateRoot
    {
        public Guid SessionId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid AccountId { get; private set; }
        public string Email { get; private set; }
        public string IPAddress { get; private set; }
        public string UserAgent { get; private set; }
        public LoginMethod LoginMethod { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public DateTime? LastActivityAt { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsMfaVerified { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiresAt { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; }
        public virtual Account Account { get; private set; }

        private UserSession() { } // EF Core

        public static UserSession Create(
            Guid userId,
            Guid accountId,
            string email,
            string ipAddress,
            string userAgent,
            LoginMethod loginMethod,
            TimeSpan sessionDuration,
            bool requiresMfa = false)
        {
            var session = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = userId,
                AccountId = accountId,
                Email = email,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                LoginMethod = loginMethod,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(sessionDuration),
                LastActivityAt = DateTime.UtcNow,
                IsActive = true,
                IsMfaVerified = !requiresMfa
            };

            session.AddDomainEvent(new LoginSucceededDomainEvent(
                session.UserId,
                session.Email,
                session.AccountId,
                session.IPAddress,
                session.UserAgent,
                session.LoginMethod,
                session.SessionId));

            return session;
        }

        public void UpdateActivity()
        {
            LastActivityAt = DateTime.UtcNow;
        }

        public void SetRefreshToken(string refreshToken, TimeSpan refreshTokenDuration)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiresAt = DateTime.UtcNow.Add(refreshTokenDuration);
        }

        public void CompleteMfaVerification()
        {
            IsMfaVerified = true;
        }

        public void Expire()
        {
            IsActive = false;
            var sessionDuration = DateTime.UtcNow - CreatedAt;

            AddDomainEvent(new SessionExpiredDomainEvent(
                UserId,
                AccountId,
                SessionId,
                sessionDuration));
        }

        public void Logout(LogoutType logoutType)
        {
            IsActive = false;

            AddDomainEvent(new LogoutInitiatedDomainEvent(
                UserId,
                AccountId,
                SessionId,
                logoutType));
        }

        public bool IsExpired()
        {
            return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
        }

        public bool IsRefreshTokenValid()
        {
            return !string.IsNullOrEmpty(RefreshToken) &&
                   RefreshTokenExpiresAt.HasValue &&
                   DateTime.UtcNow < RefreshTokenExpiresAt.Value;
        }
    }
}
```

### PasswordResetToken (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class PasswordResetToken : AggregateRoot
    {
        public Guid TokenId { get; private set; }
        public string Email { get; private set; }
        public string TokenHash { get; private set; }
        public string IPAddress { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public Guid? UserId { get; private set; }

        // Navigation properties
        public virtual User? User { get; private set; }

        private PasswordResetToken() { } // EF Core

        public static PasswordResetToken Create(
            string email,
            string tokenHash,
            string ipAddress,
            TimeSpan validityDuration)
        {
            var token = new PasswordResetToken
            {
                TokenId = Guid.NewGuid(),
                Email = email,
                TokenHash = tokenHash,
                IPAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(validityDuration),
                IsUsed = false
            };

            token.AddDomainEvent(new PasswordResetRequestedDomainEvent(
                token.Email,
                token.IPAddress,
                token.TokenHash));

            return token;
        }

        public void MarkAsUsed(Guid userId, Guid accountId)
        {
            if (IsUsed)
                throw new InvalidOperationException("Reset token has already been used");

            if (DateTime.UtcNow > ExpiresAt)
                throw new InvalidOperationException("Reset token has expired");

            IsUsed = true;
            UsedAt = DateTime.UtcNow;
            UserId = userId;

            AddDomainEvent(new PasswordResetCompletedDomainEvent(
                userId,
                accountId,
                ResetMethod.Email));
        }

        public bool IsValid()
        {
            return !IsUsed && DateTime.UtcNow <= ExpiresAt;
        }
    }
}
```

### MfaConfiguration (Aggregate Root)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class MfaConfiguration : AggregateRoot
    {
        public Guid MfaConfigId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid AccountId { get; private set; }
        public MfaMethod MfaMethod { get; private set; }
        public bool IsEnabled { get; private set; }
        public string? EncryptedSecret { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? EmailAddress { get; private set; }
        public DateTime? EnabledAt { get; private set; }
        public DateTime? DisabledAt { get; private set; }
        public Guid? EnabledBy { get; private set; }
        public Guid? DisabledBy { get; private set; }
        public List<string> BackupCodes { get; private set; } = new();
        public int FailedAttempts { get; private set; }
        public DateTime? LastVerifiedAt { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; }
        public virtual Account Account { get; private set; }

        private MfaConfiguration() { } // EF Core

        public static MfaConfiguration Create(
            Guid userId,
            Guid accountId,
            MfaMethod mfaMethod,
            Guid enabledBy,
            string? encryptedSecret = null,
            string? phoneNumber = null,
            string? emailAddress = null)
        {
            var config = new MfaConfiguration
            {
                MfaConfigId = Guid.NewGuid(),
                UserId = userId,
                AccountId = accountId,
                MfaMethod = mfaMethod,
                IsEnabled = true,
                EncryptedSecret = encryptedSecret,
                PhoneNumber = phoneNumber,
                EmailAddress = emailAddress,
                EnabledAt = DateTime.UtcNow,
                EnabledBy = enabledBy,
                FailedAttempts = 0
            };

            config.AddDomainEvent(new MfaEnabledDomainEvent(
                config.UserId,
                config.AccountId,
                config.MfaMethod,
                config.EnabledBy.Value));

            return config;
        }

        public void Disable(Guid disabledBy, string reason)
        {
            IsEnabled = false;
            DisabledAt = DateTime.UtcNow;
            DisabledBy = disabledBy;

            AddDomainEvent(new MfaDisabledDomainEvent(
                UserId,
                AccountId,
                disabledBy,
                reason));
        }

        public void GenerateBackupCodes(List<string> hashedBackupCodes)
        {
            BackupCodes = hashedBackupCodes;
        }

        public void RecordFailedAttempt()
        {
            FailedAttempts++;
        }

        public void RecordSuccessfulVerification()
        {
            FailedAttempts = 0;
            LastVerifiedAt = DateTime.UtcNow;
        }

        public void ResetFailedAttempts()
        {
            FailedAttempts = 0;
        }
    }
}
```

### LoginAttempt (Entity)

```csharp
namespace StartupStarter.Domain.Entities
{
    public class LoginAttempt
    {
        public Guid AttemptId { get; private set; }
        public string Email { get; private set; }
        public Guid? UserId { get; private set; }
        public string IPAddress { get; private set; }
        public string UserAgent { get; private set; }
        public LoginMethod LoginMethod { get; private set; }
        public bool IsSuccessful { get; private set; }
        public FailureReason? FailureReason { get; private set; }
        public DateTime AttemptedAt { get; private set; }

        // Navigation
        public virtual User? User { get; private set; }

        private LoginAttempt() { } // EF Core

        public static LoginAttempt CreateSuccessful(
            string email,
            Guid userId,
            string ipAddress,
            string userAgent,
            LoginMethod loginMethod)
        {
            return new LoginAttempt
            {
                AttemptId = Guid.NewGuid(),
                Email = email,
                UserId = userId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                LoginMethod = loginMethod,
                IsSuccessful = true,
                AttemptedAt = DateTime.UtcNow
            };
        }

        public static LoginAttempt CreateFailed(
            string email,
            string ipAddress,
            string userAgent,
            FailureReason failureReason,
            Guid? userId = null)
        {
            return new LoginAttempt
            {
                AttemptId = Guid.NewGuid(),
                Email = email,
                UserId = userId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                LoginMethod = LoginMethod.Password,
                IsSuccessful = false,
                FailureReason = failureReason,
                AttemptedAt = DateTime.UtcNow
            };
        }
    }
}
```

## Enumerations

### LoginMethod

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum LoginMethod
    {
        Password = 0,
        SSO = 1,
        OAuth = 2,
        MFA = 3
    }
}
```

### FailureReason

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum FailureReason
    {
        InvalidCredentials = 0,
        AccountLocked = 1,
        AccountDisabled = 2,
        MFAFailed = 3
    }
}
```

### LogoutType

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum LogoutType
    {
        Manual = 0,
        SessionExpired = 1,
        ForcedLogout = 2
    }
}
```

### MfaMethod

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum MfaMethod
    {
        SMS = 0,
        Email = 1,
        AuthenticatorApp = 2,
        HardwareToken = 3
    }
}
```

### ResetMethod

```csharp
namespace StartupStarter.Domain.Enums
{
    public enum ResetMethod
    {
        Email = 0,
        AdminReset = 1,
        SecurityQuestions = 2
    }
}
```

## Domain Events

### LoginAttemptedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record LoginAttemptedDomainEvent(
        Guid? UserId,
        string Email,
        string IPAddress,
        string UserAgent,
        LoginMethod LoginMethod) : DomainEvent;
}
```

### LoginSucceededDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record LoginSucceededDomainEvent(
        Guid UserId,
        string Email,
        Guid AccountId,
        string IPAddress,
        string UserAgent,
        LoginMethod LoginMethod,
        Guid SessionId) : DomainEvent;
}
```

### LoginFailedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record LoginFailedDomainEvent(
        string Email,
        string IPAddress,
        string UserAgent,
        FailureReason FailureReason,
        int AttemptCount) : DomainEvent;
}
```

### LogoutInitiatedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record LogoutInitiatedDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid SessionId,
        LogoutType LogoutType) : DomainEvent;
}
```

### SessionExpiredDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record SessionExpiredDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid SessionId,
        TimeSpan SessionDuration) : DomainEvent;
}
```

### MfaEnabledDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MfaEnabledDomainEvent(
        Guid UserId,
        Guid AccountId,
        MfaMethod MfaMethod,
        Guid EnabledBy) : DomainEvent;
}
```

### MfaDisabledDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record MfaDisabledDomainEvent(
        Guid UserId,
        Guid AccountId,
        Guid DisabledBy,
        string Reason) : DomainEvent;
}
```

### PasswordResetRequestedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record PasswordResetRequestedDomainEvent(
        string Email,
        string IPAddress,
        string TokenHash) : DomainEvent;
}
```

### PasswordResetCompletedDomainEvent

```csharp
namespace StartupStarter.Domain.Events
{
    public record PasswordResetCompletedDomainEvent(
        Guid UserId,
        Guid AccountId,
        ResetMethod ResetMethod) : DomainEvent;
}
```

## MediatR Commands

### LoginCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record LoginCommand(
        string Email,
        string Password,
        string IPAddress,
        string UserAgent) : IRequest<LoginResponse>;

    public record LoginResponse(
        bool Success,
        string? AccessToken,
        string? RefreshToken,
        bool RequiresMfa,
        Guid? SessionId,
        DateTime? ExpiresAt,
        FailureReason? FailureReason = null);
}
```

### LoginWithMfaCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record LoginWithMfaCommand(
        Guid SessionId,
        string MfaCode,
        string IPAddress,
        string UserAgent) : IRequest<LoginResponse>;
}
```

### LogoutCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record LogoutCommand(
        Guid SessionId,
        LogoutType LogoutType = LogoutType.Manual) : IRequest<Unit>;
}
```

### RefreshTokenCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record RefreshTokenCommand(
        string RefreshToken,
        string IPAddress,
        string UserAgent) : IRequest<RefreshTokenResponse>;

    public record RefreshTokenResponse(
        bool Success,
        string? AccessToken,
        string? RefreshToken,
        DateTime? ExpiresAt);
}
```

### RequestPasswordResetCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record RequestPasswordResetCommand(
        string Email,
        string IPAddress) : IRequest<RequestPasswordResetResponse>;

    public record RequestPasswordResetResponse(
        bool Success,
        string Message);
}
```

### ResetPasswordCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record ResetPasswordCommand(
        string Token,
        string NewPassword,
        string ConfirmPassword) : IRequest<ResetPasswordResponse>;

    public record ResetPasswordResponse(
        bool Success,
        string Message);
}
```

### EnableMfaCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record EnableMfaCommand(
        Guid UserId,
        Guid AccountId,
        MfaMethod MfaMethod,
        Guid EnabledBy,
        string? PhoneNumber = null,
        string? EmailAddress = null) : IRequest<EnableMfaResponse>;

    public record EnableMfaResponse(
        bool Success,
        string? QrCodeUri,
        string? Secret,
        List<string>? BackupCodes);
}
```

### DisableMfaCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record DisableMfaCommand(
        Guid UserId,
        Guid DisabledBy,
        string Reason) : IRequest<Unit>;
}
```

### VerifyMfaCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record VerifyMfaCommand(
        Guid UserId,
        string Code,
        MfaMethod Method) : IRequest<VerifyMfaResponse>;

    public record VerifyMfaResponse(
        bool Success,
        string? Message);
}
```

### RevokeSessionCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record RevokeSessionCommand(
        Guid SessionId,
        Guid RevokedBy) : IRequest<Unit>;
}
```

### RevokeAllSessionsCommand

```csharp
namespace StartupStarter.Application.Authentication.Commands
{
    public record RevokeAllSessionsCommand(
        Guid UserId,
        Guid? ExceptSessionId = null) : IRequest<int>;
}
```

## MediatR Queries

### GetUserSessionQuery

```csharp
namespace StartupStarter.Application.Authentication.Queries
{
    public record GetUserSessionQuery(Guid SessionId) : IRequest<UserSessionDto?>;
}
```

### GetActiveSessionsQuery

```csharp
namespace StartupStarter.Application.Authentication.Queries
{
    public record GetActiveSessionsQuery(
        Guid UserId,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<UserSessionDto>>;
}
```

### GetLoginHistoryQuery

```csharp
namespace StartupStarter.Application.Authentication.Queries
{
    public record GetLoginHistoryQuery(
        Guid UserId,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        int PageNumber = 1,
        int PageSize = 20) : IRequest<PaginatedList<LoginAttemptDto>>;
}
```

### GetMfaConfigurationQuery

```csharp
namespace StartupStarter.Application.Authentication.Queries
{
    public record GetMfaConfigurationQuery(Guid UserId) : IRequest<MfaConfigurationDto?>;
}
```

### ValidateSessionQuery

```csharp
namespace StartupStarter.Application.Authentication.Queries
{
    public record ValidateSessionQuery(Guid SessionId) : IRequest<SessionValidationDto>;
}
```

## DTOs

### UserSessionDto

```csharp
namespace StartupStarter.Application.Authentication.DTOs
{
    public record UserSessionDto(
        Guid SessionId,
        Guid UserId,
        string Email,
        string IPAddress,
        string UserAgent,
        LoginMethod LoginMethod,
        DateTime CreatedAt,
        DateTime? ExpiresAt,
        DateTime? LastActivityAt,
        bool IsActive,
        bool IsMfaVerified);
}
```

### LoginAttemptDto

```csharp
namespace StartupStarter.Application.Authentication.DTOs
{
    public record LoginAttemptDto(
        Guid AttemptId,
        string Email,
        string IPAddress,
        string UserAgent,
        LoginMethod LoginMethod,
        bool IsSuccessful,
        FailureReason? FailureReason,
        DateTime AttemptedAt);
}
```

### MfaConfigurationDto

```csharp
namespace StartupStarter.Application.Authentication.DTOs
{
    public record MfaConfigurationDto(
        Guid MfaConfigId,
        Guid UserId,
        MfaMethod MfaMethod,
        bool IsEnabled,
        string? PhoneNumber,
        string? EmailAddress,
        DateTime? EnabledAt,
        DateTime? LastVerifiedAt,
        int BackupCodesRemaining);
}
```

### SessionValidationDto

```csharp
namespace StartupStarter.Application.Authentication.DTOs
{
    public record SessionValidationDto(
        bool IsValid,
        bool IsExpired,
        bool RequiresMfaVerification,
        DateTime? ExpiresAt,
        string? Message);
}
```

## Repository Interfaces

### IUserSessionRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<UserSession?> GetActiveSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<List<UserSession>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<PaginatedList<UserSession>> GetPagedActiveSessionsAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default);
        Task RevokeAllSessionsAsync(Guid userId, Guid? exceptSessionId = null, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IPasswordResetTokenRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<PasswordResetToken?> GetValidTokenByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
        Task UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
        Task InvalidateAllTokensForEmailAsync(string email, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### IMfaConfigurationRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IMfaConfigurationRepository
    {
        Task<MfaConfiguration?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<MfaConfiguration?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(MfaConfiguration configuration, CancellationToken cancellationToken = default);
        Task UpdateAsync(MfaConfiguration configuration, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

### ILoginAttemptRepository

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface ILoginAttemptRepository
    {
        Task<LoginAttempt?> GetByIdAsync(Guid attemptId, CancellationToken cancellationToken = default);
        Task<List<LoginAttempt>> GetRecentAttemptsByEmailAsync(
            string email,
            TimeSpan timeWindow,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<LoginAttempt>> GetPagedByUserIdAsync(
            Guid userId,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
        Task<int> GetFailedAttemptsCountAsync(
            string email,
            TimeSpan timeWindow,
            CancellationToken cancellationToken = default);
        Task AddAsync(LoginAttempt attempt, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
```

## Service Interfaces

### IJwtService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, Guid accountId, string email, Dictionary<string, string>? claims = null);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiration(string token);
        Guid? GetUserIdFromToken(string token);
    }
}
```

### IMfaService

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IMfaService
    {
        string GenerateSecret();
        string GenerateQrCodeUri(string email, string secret, string issuer);
        bool ValidateCode(string secret, string code);
        List<string> GenerateBackupCodes(int count = 10);
        bool ValidateBackupCode(string hashedCode, string providedCode);
        Task SendMfaCodeBySmsAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
        Task SendMfaCodeByEmailAsync(string email, string code, CancellationToken cancellationToken = default);
    }
}
```

### IPasswordHasher

```csharp
namespace StartupStarter.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
        string GenerateSecureToken();
        string HashToken(string token);
    }
}
```
