namespace StartupStarter.Core.Model.ContentAggregate.Entities;

public class ContentVersion
{
    public string ContentVersionId { get; private set; }
    public string ContentId { get; private set; }
    public int VersionNumber { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public string CreatedBy { get; private set; }
    public string ChangeDescription { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Content Content { get; private set; } = null!;

    // EF Core constructor
    private ContentVersion()
    {
        ContentVersionId = string.Empty;
        ContentId = string.Empty;
        Title = string.Empty;
        Body = string.Empty;
        CreatedBy = string.Empty;
        ChangeDescription = string.Empty;
    }

    public ContentVersion(string contentVersionId, string contentId, int versionNumber,
        string title, string body, string createdBy, string changeDescription)
    {
        if (string.IsNullOrWhiteSpace(contentVersionId))
            throw new ArgumentException("ContentVersion ID cannot be empty", nameof(contentVersionId));
        if (string.IsNullOrWhiteSpace(contentId))
            throw new ArgumentException("Content ID cannot be empty", nameof(contentId));
        if (versionNumber < 1)
            throw new ArgumentException("Version number must be greater than 0", nameof(versionNumber));
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be empty", nameof(createdBy));

        ContentVersionId = contentVersionId;
        ContentId = contentId;
        VersionNumber = versionNumber;
        Title = title ?? string.Empty;
        Body = body ?? string.Empty;
        CreatedBy = createdBy;
        ChangeDescription = changeDescription ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }
}
