using MediatR;
using StartupStarter.Api.Features.ContentManagement.Dtos;

namespace StartupStarter.Api.Features.ContentManagement.Commands;

public class CreateContentCommand : IRequest<ContentDto>
{
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
}
