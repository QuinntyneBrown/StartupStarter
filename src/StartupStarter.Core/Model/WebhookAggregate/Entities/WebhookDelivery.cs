namespace StartupStarter.Core.Model.WebhookAggregate.Entities;

public class WebhookDelivery
{
    public string WebhookDeliveryId { get; private set; }
    public string WebhookId { get; private set; }
    public string EventType { get; private set; }
    public string PayloadJson { get; private set; }
    public int ResponseStatus { get; private set; }
    public bool Success { get; private set; }
    public string FailureReason { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Webhook Webhook { get; private set; } = null!;

    // EF Core constructor
    private WebhookDelivery()
    {
        WebhookDeliveryId = string.Empty;
        WebhookId = string.Empty;
        EventType = string.Empty;
        PayloadJson = string.Empty;
        FailureReason = string.Empty;
    }

    public WebhookDelivery(string webhookDeliveryId, string webhookId, string eventType,
        string payloadJson, int responseStatus, bool success, string failureReason = null)
    {
        if (string.IsNullOrWhiteSpace(webhookDeliveryId))
            throw new ArgumentException("WebhookDelivery ID cannot be empty", nameof(webhookDeliveryId));
        if (string.IsNullOrWhiteSpace(webhookId))
            throw new ArgumentException("Webhook ID cannot be empty", nameof(webhookId));
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        WebhookDeliveryId = webhookDeliveryId;
        WebhookId = webhookId;
        EventType = eventType;
        PayloadJson = payloadJson ?? string.Empty;
        ResponseStatus = responseStatus;
        Success = success;
        FailureReason = failureReason ?? string.Empty;
        RetryCount = 0;
        Timestamp = DateTime.UtcNow;
    }

    public void IncrementRetry()
    {
        RetryCount++;
    }
}
