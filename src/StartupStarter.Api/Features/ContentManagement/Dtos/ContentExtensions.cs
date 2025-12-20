using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Api.Features.ContentManagement.Dtos;

public static class ContentExtensions
{
    public static ContentDto ToDto(this Content content)
    {
        return new ContentDto
        {
            ContentId = content.ContentId,
            ProfileId = content.ProfileId,
            Title = content.Title,
            Body = content.Body,
            ContentType = content.ContentType,
            Status = content.Status.ToString(),
            CreatedAt = content.CreatedAt,
            UpdatedAt = content.UpdatedAt,
            PublishedAt = content.PublishedAt,
            ScheduledAt = content.ScheduledPublishDate,
            DeletedAt = content.DeletedAt,
            DeletionType = content.DeletionType?.ToString()
        };
    }

    public static ContentVersionDto ToDto(this ContentVersion contentVersion)
    {
        return new ContentVersionDto
        {
            VersionId = contentVersion.ContentVersionId,
            ContentId = contentVersion.ContentId,
            VersionNumber = contentVersion.VersionNumber,
            Title = contentVersion.Title,
            Body = contentVersion.Body,
            CreatedAt = contentVersion.CreatedAt,
            CreatedBy = contentVersion.CreatedBy
        };
    }
}
