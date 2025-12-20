using StartupStarter.Core.Model.UserAggregate.Events;

namespace StartupStarter.Core.Model.UserAggregate.Entities;

public class UserInvitation
{
    public string InvitationId { get; private set; }
    public string Email { get; private set; }
    public string AccountId { get; private set; }
    public string InvitedBy { get; private set; }
    public DateTime SentAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public string? AcceptedByUserId { get; private set; }
    public bool IsAccepted { get; private set; }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    private readonly List<string> _roleIds = new();
    public IReadOnlyCollection<string> RoleIds => _roleIds.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private UserInvitation() { }

    public UserInvitation(string invitationId, string email, string accountId,
        string invitedBy, List<string> roleIds, DateTime expiresAt)
    {
        InvitationId = invitationId;
        Email = email;
        AccountId = accountId;
        InvitedBy = invitedBy;
        SentAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsAccepted = false;
        _roleIds.AddRange(roleIds);

        AddDomainEvent(new UserInvitationSentEvent
        {
            InvitationId = InvitationId,
            Email = Email,
            AccountId = AccountId,
            InvitedBy = InvitedBy,
            Roles = roleIds,
            ExpiresAt = ExpiresAt,
            Timestamp = SentAt
        });
    }

    public void Accept(string userId)
    {
        if (IsExpired)
            throw new InvalidOperationException("Invitation has expired");
        if (IsAccepted)
            throw new InvalidOperationException("Invitation already accepted");

        IsAccepted = true;
        AcceptedAt = DateTime.UtcNow;
        AcceptedByUserId = userId;

        AddDomainEvent(new UserInvitationAcceptedEvent
        {
            InvitationId = InvitationId,
            UserId = userId,
            Email = Email,
            AccountId = AccountId,
            Timestamp = AcceptedAt.Value
        });
    }

    public void MarkExpired()
    {
        if (!IsExpired)
            return;

        AddDomainEvent(new UserInvitationExpiredEvent
        {
            InvitationId = InvitationId,
            Email = Email,
            AccountId = AccountId,
            Timestamp = DateTime.UtcNow
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
