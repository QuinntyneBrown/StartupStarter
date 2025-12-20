using MediatR;
using StartupStarter.Api.Features.MediaManagement.Dtos;

namespace StartupStarter.Api.Features.MediaManagement.Commands;

public class UploadMediaCommand : IRequest<MediaDto>
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
}
