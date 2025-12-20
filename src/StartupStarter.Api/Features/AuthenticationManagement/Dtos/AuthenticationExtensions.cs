using StartupStarter.Core.Model.AuthenticationAggregate.Entities;

namespace StartupStarter.Api.Features.AuthenticationManagement.Dtos;

public static class AuthenticationExtensions
{
    public static UserSessionDto ToDto(this UserSession session)
    {
        return new UserSessionDto
        {
            SessionId = session.SessionId,
            UserId = session.UserId,
            Token = string.Empty, // Token not exposed in entity
            IpAddress = session.IPAddress,
            UserAgent = session.UserAgent,
            CreatedAt = session.CreatedAt,
            ExpiresAt = session.ExpiresAt,
            LastActivityAt = session.CreatedAt, // Using CreatedAt as LastActivityAt placeholder
            EndedAt = session.LoggedOutAt
        };
    }

    public static LoginAttemptDto ToDto(this LoginAttempt attempt)
    {
        return new LoginAttemptDto
        {
            AttemptId = attempt.LoginAttemptId,
            UserId = attempt.UserId,
            Success = attempt.Success,
            IpAddress = attempt.IPAddress,
            UserAgent = attempt.UserAgent,
            AttemptedAt = attempt.Timestamp,
            FailureReason = attempt.FailureReason?.ToString()
        };
    }

    public static MfaDto ToDto(this MultiFactorAuthentication mfa)
    {
        return new MfaDto
        {
            MfaId = mfa.MfaId,
            UserId = mfa.UserId,
            MfaType = mfa.Method.ToString(),
            Secret = mfa.SecretKey,
            BackupCodes = mfa.BackupCodesJson,
            IsEnabled = mfa.IsEnabled,
            EnabledAt = mfa.EnabledAt,
            DisabledAt = mfa.DisabledAt
        };
    }

    public static PasswordResetRequestDto ToDto(this PasswordResetRequest request)
    {
        return new PasswordResetRequestDto
        {
            RequestId = request.ResetRequestId,
            UserId = request.UserId,
            Token = request.ResetTokenHash,
            RequestedAt = request.RequestedAt,
            ExpiresAt = request.ExpiresAt,
            CompletedAt = request.CompletedAt,
            IpAddress = request.IPAddress
        };
    }
}
