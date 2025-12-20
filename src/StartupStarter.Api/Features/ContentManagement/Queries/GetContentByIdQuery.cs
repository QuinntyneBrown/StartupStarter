using MediatR;
using StartupStarter.Api.Features.ContentManagement.Dtos;

namespace StartupStarter.Api.Features.ContentManagement.Queries;

public class GetContentByIdQuery : IRequest<ContentDto?>
{
    public string ContentId { get; set; } = string.Empty;
}
