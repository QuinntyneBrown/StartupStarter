using MediatR;
using StartupStarter.Api.Features.MediaManagement.Dtos;

namespace StartupStarter.Api.Features.MediaManagement.Queries;

public class GetMediaByIdQuery : IRequest<MediaDto?>
{
    public string MediaId { get; set; } = string.Empty;
}
