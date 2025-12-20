using StartupStarter.Core.Model.AuthenticationAggregate.Enums;
using StartupStarter.Core.Model.AuthenticationAggregate.Events;

namespace StartupStarter.Core.Model.AuthenticationAggregate.Entities;

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

    // EF Core constructor
    private PasswordResetRequest()
    {
        ResetRequestId = string.Empty;
        UserId = string.Empty;
        Email = string.Empty;
        IPAddress = string.Empty;
        ResetTokenHash = string.Empty;
    }

    public PasswordResetRequest(string resetRequestId, string userId, string email, string ipAddress,
        string resetTokenHash, int expirationMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(resetRequestId))
            throw new ArgumentException("ResetRequest ID cannot be empty", nameof(resetRequestId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(resetTokenHash))
            throw new ArgumentException("Reset token hash cannot be empty", nameof(resetTokenHash));

        ResetRequestId = resetRequestId;
        UserId = userId ?? string.Empty;
        Email = email;
        IPAddress = ipAddress ?? string.Empty;
        ResetTokenHash = resetTokenHash;
        RequestedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        IsCompleted = false;

        AddDomainEvent(new UserPasswordResetRequestedEvent
        {
            Email = email,
            IPAddress = ipAddress ?? string.Empty,
            Timestamp = RequestedAt,
            ResetToken = resetTokenHash
        });
    }

    public void Complete(ResetMethod method)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Password reset has already been completed");
        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationException("Password reset request has expired");

        CompletedAt = DateTime.UtcNow;
        ResetMethod = method;
        IsCompleted = true;

        AddDomainEvent(new UserPasswordResetCompletedEvent
        {
            UserId = UserId,
            AccountId = string.Empty, // AccountId would need to be added if required
            Timestamp = CompletedAt.Value,
            ResetMethod = method
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
