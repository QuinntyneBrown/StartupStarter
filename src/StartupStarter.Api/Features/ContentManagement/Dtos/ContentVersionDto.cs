namespace StartupStarter.Api.Features.ContentManagement.Dtos;

public class ContentVersionDto
{
    public string VersionId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
