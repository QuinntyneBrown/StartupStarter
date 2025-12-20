namespace StartupStarter.Api.Features.WebhookManagement.Dtos;

public class WebhookDeliveryDto
{
    public string DeliveryId { get; set; } = string.Empty;
    public string WebhookId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public int HttpStatus { get; set; }
    public DateTime DeliveredAt { get; set; }
    public string? FailureReason { get; set; }
}
