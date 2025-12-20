namespace StartupStarter.Api.Features.MediaManagement.Dtos;

public class MediaDto
{
    public string MediaId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string AltText { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Categories { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
