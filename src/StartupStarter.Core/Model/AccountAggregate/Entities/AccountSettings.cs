namespace StartupStarter.Core.Model.AccountAggregate.Entities;

public class AccountSettings
{
    public string AccountSettingsId { get; private set; }
    public string AccountId { get; private set; }
    public string Category { get; private set; }
    public string SettingsJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Account Account { get; private set; } = null!;

    // EF Core constructor
    private AccountSettings() { }

    public AccountSettings(string accountSettingsId, string accountId, string category, string settingsJson)
    {
        AccountSettingsId = accountSettingsId;
        AccountId = accountId;
        Category = category;
        SettingsJson = settingsJson;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string settingsJson)
    {
        SettingsJson = settingsJson;
        UpdatedAt = DateTime.UtcNow;
    }
}
