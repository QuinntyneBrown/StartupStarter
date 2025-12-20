using MediatR;
using StartupStarter.Api.Features.DashboardManagement.Dtos;

namespace StartupStarter.Api.Features.DashboardManagement.Commands;

public class AddCardCommand : IRequest<DashboardCardDto>
{
    public string DashboardId { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string ConfigurationJson { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string AddedBy { get; set; } = string.Empty;
}
