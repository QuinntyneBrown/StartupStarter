using StartupStarter.Core.Model.ApiKeyAggregate.Events;

namespace StartupStarter.Core.Model.ApiKeyAggregate.Entities;

public class ApiKey
{
    public string ApiKeyId { get; private set; }
    public string KeyName { get; private set; }
    public string AccountId { get; private set; }
    public string KeyHash { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevocationReason { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<string> _permissions = new();
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private ApiKey()
    {
        ApiKeyId = string.Empty;
        KeyName = string.Empty;
        AccountId = string.Empty;
        KeyHash = string.Empty;
        CreatedBy = string.Empty;
    }

    public ApiKey(string apiKeyId, string keyName, string accountId, string keyHash,
        string createdBy, List<string> permissions, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(apiKeyId))
            throw new ArgumentException("API Key ID cannot be empty", nameof(apiKeyId));
        if (string.IsNullOrWhiteSpace(keyName))
            throw new ArgumentException("Key name cannot be empty", nameof(keyName));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(keyHash))
            throw new ArgumentException("Key hash cannot be empty", nameof(keyHash));
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be empty", nameof(createdBy));

        ApiKeyId = apiKeyId;
        KeyName = keyName;
        AccountId = accountId;
        KeyHash = keyHash;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;

        if (permissions != null && permissions.Any())
        {
            _permissions.AddRange(permissions);
        }

        AddDomainEvent(new ApiKeyCreatedEvent
        {
            ApiKeyId = ApiKeyId,
            KeyName = KeyName,
            AccountId = AccountId,
            CreatedBy = CreatedBy,
            Permissions = permissions ?? new List<string>(),
            ExpiresAt = ExpiresAt,
            Timestamp = CreatedAt
        });
    }

    public void Revoke(string revokedBy, string reason)
    {
        if (string.IsNullOrWhiteSpace(revokedBy))
            throw new ArgumentException("RevokedBy cannot be empty", nameof(revokedBy));

        RevokedAt = DateTime.UtcNow;
        RevokedBy = revokedBy;
        RevocationReason = reason ?? string.Empty;
        IsActive = false;

        AddDomainEvent(new ApiKeyRevokedEvent
        {
            ApiKeyId = ApiKeyId,
            AccountId = AccountId,
            RevokedBy = revokedBy,
            Reason = reason ?? string.Empty,
            Timestamp = RevokedAt.Value
        });
    }

    public void AddPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission cannot be empty", nameof(permission));

        if (!_permissions.Contains(permission))
        {
            _permissions.Add(permission);
        }
    }

    public void RemovePermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission cannot be empty", nameof(permission));

        _permissions.Remove(permission);
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
