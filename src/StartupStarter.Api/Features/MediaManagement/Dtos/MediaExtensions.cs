using StartupStarter.Core.Model.MediaAggregate.Entities;

namespace StartupStarter.Api.Features.MediaManagement.Dtos;

public static class MediaExtensions
{
    public static MediaDto ToDto(this Media media)
    {
        return new MediaDto
        {
            MediaId = media.MediaId,
            ProfileId = media.ProfileId,
            FileName = media.FileName,
            FileUrl = media.StorageLocation,
            MimeType = media.FileType,
            FileSize = media.FileSize,
            AltText = string.Empty,
            Tags = string.Join(",", media.Tags),
            Categories = string.Join(",", media.Categories),
            UploadedAt = media.UploadedAt,
            UpdatedAt = media.UpdatedAt,
            DeletedAt = media.DeletedAt
        };
    }
}
