namespace StartupStarter.Core.Model.MediaAggregate.Events;

public class MediaCategorizedEvent : DomainEvent
{
    public string MediaId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public string CategorizedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
