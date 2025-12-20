namespace StartupStarter.Api.Features.DashboardManagement.Dtos;

public class DashboardCardDto
{
    public string CardId { get; set; } = string.Empty;
    public string DashboardId { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string Configuration { get; set; } = string.Empty;
    public int Position { get; set; }
    public int Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
