using MediatR;
using StartupStarter.Api.Features.MediaManagement.Dtos;

namespace StartupStarter.Api.Features.MediaManagement.Commands;

public class UpdateMediaCommand : IRequest<MediaDto?>
{
    public string MediaId { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedFields { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
}
