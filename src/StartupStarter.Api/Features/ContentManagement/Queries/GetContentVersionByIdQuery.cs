using MediatR;
using StartupStarter.Api.Features.ContentManagement.Dtos;

namespace StartupStarter.Api.Features.ContentManagement.Queries;

public class GetContentVersionByIdQuery : IRequest<ContentVersionDto?>
{
    public string ContentId { get; set; } = string.Empty;
    public string VersionId { get; set; } = string.Empty;
}
