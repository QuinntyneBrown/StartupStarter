using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Api.Features.WebhookManagement.Dtos;

public static class WebhookExtensions
{
    public static WebhookDto ToDto(this Webhook webhook)
    {
        return new WebhookDto
        {
            WebhookId = webhook.WebhookId,
            AccountId = webhook.AccountId,
            Url = webhook.Url,
            EventTypes = string.Join(",", webhook.Events),
            Secret = string.Empty, // Don't expose secret in DTO
            IsActive = webhook.IsActive,
            CreatedAt = webhook.RegisteredAt,
            UpdatedAt = null, // Not tracked in entity
            DeletedAt = webhook.DeletedAt
        };
    }

    public static WebhookDeliveryDto ToDto(this WebhookDelivery delivery)
    {
        return new WebhookDeliveryDto
        {
            DeliveryId = delivery.WebhookDeliveryId,
            WebhookId = delivery.WebhookId,
            EventType = delivery.EventType,
            Payload = delivery.PayloadJson,
            HttpStatus = delivery.ResponseStatus,
            DeliveredAt = delivery.Timestamp,
            FailureReason = delivery.FailureReason
        };
    }
}
