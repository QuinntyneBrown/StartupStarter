using StartupStarter.Core.Model.ContentAggregate.Enums;

namespace StartupStarter.Api.Features.ContentManagement.Dtos;

public class ContentDto
{
    public string ContentId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletionType { get; set; }
}
