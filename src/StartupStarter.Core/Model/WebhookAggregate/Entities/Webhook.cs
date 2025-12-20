using StartupStarter.Core.Model.WebhookAggregate.Events;

namespace StartupStarter.Core.Model.WebhookAggregate.Entities;

public class Webhook
{
    public string WebhookId { get; private set; }
    public string Url { get; private set; }
    public string AccountId { get; private set; }
    public string RegisteredBy { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string DeletedBy { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<string> _events = new();
    public IReadOnlyCollection<string> Events => _events.AsReadOnly();

    private readonly List<WebhookDelivery> _deliveries = new();
    public IReadOnlyCollection<WebhookDelivery> Deliveries => _deliveries.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private Webhook()
    {
        WebhookId = string.Empty;
        Url = string.Empty;
        AccountId = string.Empty;
        RegisteredBy = string.Empty;
        DeletedBy = string.Empty;
    }

    public Webhook(string webhookId, string url, string accountId, string registeredBy, List<string> events)
    {
        if (string.IsNullOrWhiteSpace(webhookId))
            throw new ArgumentException("Webhook ID cannot be empty", nameof(webhookId));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(registeredBy))
            throw new ArgumentException("RegisteredBy cannot be empty", nameof(registeredBy));

        WebhookId = webhookId;
        Url = url;
        AccountId = accountId;
        RegisteredBy = registeredBy;
        RegisteredAt = DateTime.UtcNow;
        DeletedBy = string.Empty;
        IsActive = true;

        if (events != null && events.Any())
        {
            _events.AddRange(events);
        }

        AddDomainEvent(new WebhookRegisteredEvent
        {
            WebhookId = WebhookId,
            Url = Url,
            Events = events ?? new List<string>(),
            AccountId = AccountId,
            RegisteredBy = RegisteredBy,
            Timestamp = RegisteredAt
        });
    }

    public void RegisterEvent(string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        if (!_events.Contains(eventType))
        {
            _events.Add(eventType);
        }
    }

    public void UnregisterEvent(string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        _events.Remove(eventType);
    }

    public void Delete(string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy cannot be empty", nameof(deletedBy));

        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;

        AddDomainEvent(new WebhookDeletedEvent
        {
            WebhookId = WebhookId,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            Timestamp = DeletedAt.Value
        });
    }

    public void RecordDelivery(string eventType, object payload, int responseStatus, bool success, string failureReason = null)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        var timestamp = DateTime.UtcNow;

        if (success)
        {
            AddDomainEvent(new WebhookTriggeredEvent
            {
                WebhookId = WebhookId,
                AccountId = AccountId,
                EventType = eventType,
                PayloadSent = payload,
                ResponseStatus = responseStatus,
                Timestamp = timestamp
            });
        }
        else
        {
            // Count retries for this delivery
            var retryCount = _deliveries.Count(d => d.EventType == eventType && !d.Success);

            AddDomainEvent(new WebhookFailedEvent
            {
                WebhookId = WebhookId,
                AccountId = AccountId,
                EventType = eventType,
                FailureReason = failureReason ?? string.Empty,
                RetryCount = retryCount,
                Timestamp = timestamp
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
