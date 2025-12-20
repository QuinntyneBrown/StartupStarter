namespace StartupStarter.Api.Features.WebhookManagement.Dtos;

public class WebhookDto
{
    public string WebhookId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string EventTypes { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
