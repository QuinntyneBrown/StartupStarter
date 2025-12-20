namespace StartupStarter.Core.Model.AccountAggregate.Events;

public class AccountSettingsUpdatedEvent : DomainEvent
{
    public string AccountId { get; set; } = string.Empty;
    public string SettingCategory { get; set; } = string.Empty;
    public Dictionary<string, object> UpdatedSettings { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
