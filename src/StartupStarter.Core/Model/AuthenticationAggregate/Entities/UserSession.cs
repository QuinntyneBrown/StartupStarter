using StartupStarter.Core.Model.AuthenticationAggregate.Enums;
using StartupStarter.Core.Model.AuthenticationAggregate.Events;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Entities;

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

    // EF Core constructor
    private UserSession()
    {
        SessionId = string.Empty;
        UserId = string.Empty;
        AccountId = string.Empty;
        IPAddress = string.Empty;
        UserAgent = string.Empty;
    }

    public UserSession(string sessionId, string userId, string accountId, string ipAddress,
        string userAgent, LoginMethod loginMethod, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));

        SessionId = sessionId;
        UserId = userId;
        AccountId = accountId;
        IPAddress = ipAddress ?? string.Empty;
        UserAgent = userAgent ?? string.Empty;
        LoginMethod = loginMethod;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    public void EndSession(LogoutType logoutType)
    {
        LoggedOutAt = DateTime.UtcNow;
        LogoutType = logoutType;
        IsActive = false;

        if (logoutType == Enums.LogoutType.SessionExpired)
        {
            var sessionDuration = LoggedOutAt.Value - CreatedAt;
            AddDomainEvent(new UserSessionExpiredEvent
            {
                UserId = UserId,
                AccountId = AccountId,
                SessionId = SessionId,
                Timestamp = LoggedOutAt.Value,
                SessionDuration = sessionDuration
            });
        }
        else
        {
            AddDomainEvent(new UserLogoutInitiatedEvent
            {
                UserId = UserId,
                AccountId = AccountId,
                SessionId = SessionId,
                Timestamp = LoggedOutAt.Value,
                LogoutType = logoutType
            });
        }
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
