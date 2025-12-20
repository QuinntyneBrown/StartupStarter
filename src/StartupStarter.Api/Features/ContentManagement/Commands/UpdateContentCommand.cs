using MediatR;
using StartupStarter.Api.Features.ContentManagement.Dtos;

namespace StartupStarter.Api.Features.ContentManagement.Commands;

public class UpdateContentCommand : IRequest<ContentDto>
{
    public string ContentId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}
