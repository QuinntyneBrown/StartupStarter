namespace StartupStarter.Core.Model.ProfileAggregate.Entities;

public class ProfilePreferences
{
    public string ProfilePreferencesId { get; private set; }
    public string ProfileId { get; private set; }
    public string Category { get; private set; }
    public string PreferencesJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Profile Profile { get; private set; } = null!;

    private ProfilePreferences() { }

    public ProfilePreferences(string profilePreferencesId, string profileId, string category, string preferencesJson)
    {
        ProfilePreferencesId = profilePreferencesId;
        ProfileId = profileId;
        Category = category;
        PreferencesJson = preferencesJson;
        CreatedAt = DateTime.UtcNow;
    }
}
