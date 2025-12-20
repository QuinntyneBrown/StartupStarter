using StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

namespace StartupStarter.Core.Model.DashboardAggregate.Entities;

public class DashboardCard
{
    public string CardId { get; private set; }
    public string DashboardId { get; private set; }
    public string CardType { get; private set; }
    public string ConfigurationJson { get; private set; }
    public CardPosition Position { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Dashboard Dashboard { get; private set; } = null!;

    // EF Core constructor
    private DashboardCard()
    {
        CardId = string.Empty;
        DashboardId = string.Empty;
        CardType = string.Empty;
        ConfigurationJson = string.Empty;
        Position = null!;
    }

    public DashboardCard(string cardId, string dashboardId, string cardType,
        string configurationJson, CardPosition position)
    {
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be empty", nameof(cardId));
        if (string.IsNullOrWhiteSpace(dashboardId))
            throw new ArgumentException("Dashboard ID cannot be empty", nameof(dashboardId));
        if (string.IsNullOrWhiteSpace(cardType))
            throw new ArgumentException("Card type cannot be empty", nameof(cardType));
        if (position == null)
            throw new ArgumentException("Position cannot be null", nameof(position));

        CardId = cardId;
        DashboardId = dashboardId;
        CardType = cardType;
        ConfigurationJson = configurationJson ?? string.Empty;
        Position = position;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePosition(CardPosition newPosition)
    {
        if (newPosition == null)
            throw new ArgumentException("Position cannot be null", nameof(newPosition));

        Position = newPosition;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateConfiguration(Dictionary<string, object> config)
    {
        if (config == null || !config.Any())
            throw new ArgumentException("Configuration cannot be empty", nameof(config));

        ConfigurationJson = System.Text.Json.JsonSerializer.Serialize(config);
        UpdatedAt = DateTime.UtcNow;
    }
}
