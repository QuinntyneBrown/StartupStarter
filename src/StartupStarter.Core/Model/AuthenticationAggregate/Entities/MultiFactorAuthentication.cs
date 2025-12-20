using StartupStarter.Core.Model.AuthenticationAggregate.Enums;
using StartupStarter.Core.Model.AuthenticationAggregate.Events;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Entities;

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

    // EF Core constructor
    private MultiFactorAuthentication()
    {
        MfaId = string.Empty;
        UserId = string.Empty;
        AccountId = string.Empty;
        EnabledBy = string.Empty;
        DisabledBy = string.Empty;
        DisabledReason = string.Empty;
        SecretKey = string.Empty;
        BackupCodesJson = string.Empty;
    }

    public MultiFactorAuthentication(string mfaId, string userId, string accountId, MfaMethod method,
        string enabledBy, string secretKey = null, string backupCodesJson = null)
    {
        if (string.IsNullOrWhiteSpace(mfaId))
            throw new ArgumentException("MFA ID cannot be empty", nameof(mfaId));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(enabledBy))
            throw new ArgumentException("EnabledBy cannot be empty", nameof(enabledBy));

        MfaId = mfaId;
        UserId = userId;
        AccountId = accountId;
        Method = method;
        IsEnabled = true;
        EnabledBy = enabledBy;
        EnabledAt = DateTime.UtcNow;
        DisabledBy = string.Empty;
        DisabledReason = string.Empty;
        SecretKey = secretKey ?? string.Empty;
        BackupCodesJson = backupCodesJson ?? string.Empty;

        AddDomainEvent(new UserMfaEnabledEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            MfaMethod = method,
            EnabledBy = enabledBy,
            Timestamp = EnabledAt
        });
    }

    public void Enable(MfaMethod method, string enabledBy)
    {
        if (string.IsNullOrWhiteSpace(enabledBy))
            throw new ArgumentException("EnabledBy cannot be empty", nameof(enabledBy));

        Method = method;
        IsEnabled = true;
        EnabledBy = enabledBy;
        EnabledAt = DateTime.UtcNow;

        AddDomainEvent(new UserMfaEnabledEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            MfaMethod = method,
            EnabledBy = enabledBy,
            Timestamp = EnabledAt
        });
    }

    public void Disable(string disabledBy, string reason)
    {
        if (string.IsNullOrWhiteSpace(disabledBy))
            throw new ArgumentException("DisabledBy cannot be empty", nameof(disabledBy));

        IsEnabled = false;
        DisabledAt = DateTime.UtcNow;
        DisabledBy = disabledBy;
        DisabledReason = reason ?? string.Empty;

        AddDomainEvent(new UserMfaDisabledEvent
        {
            UserId = UserId,
            AccountId = AccountId,
            DisabledBy = disabledBy,
            Timestamp = DisabledAt.Value,
            Reason = reason ?? string.Empty
        });
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
