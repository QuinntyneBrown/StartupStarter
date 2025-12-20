using StartupStarter.Core.Model.AuthenticationAggregate.Enums;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Entities;

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

    // EF Core constructor
    private LoginAttempt()
    {
        LoginAttemptId = string.Empty;
        UserId = string.Empty;
        Email = string.Empty;
        IPAddress = string.Empty;
        UserAgent = string.Empty;
    }

    public LoginAttempt(string loginAttemptId, string userId, string email, string ipAddress,
        string userAgent, LoginMethod loginMethod, bool success, FailureReason? failureReason = null,
        int attemptCount = 1)
    {
        if (string.IsNullOrWhiteSpace(loginAttemptId))
            throw new ArgumentException("LoginAttempt ID cannot be empty", nameof(loginAttemptId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        LoginAttemptId = loginAttemptId;
        UserId = userId ?? string.Empty;
        Email = email;
        IPAddress = ipAddress ?? string.Empty;
        UserAgent = userAgent ?? string.Empty;
        LoginMethod = loginMethod;
        Success = success;
        FailureReason = failureReason;
        AttemptCount = attemptCount;
        Timestamp = DateTime.UtcNow;
    }
}
