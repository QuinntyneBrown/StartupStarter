using MediatR;
using StartupStarter.Api.Features.ContentManagement.Dtos;

namespace StartupStarter.Api.Features.ContentManagement.Commands;

public class PublishContentCommand : IRequest<ContentDto>
{
    public string ContentId { get; set; } = string.Empty;
    public string PublishedBy { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
}
