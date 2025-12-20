namespace StartupStarter.Api.Features.ProfileManagement.Dtos;

public class ProfilePreferencesDto
{
    public string PreferencesId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string? Theme { get; set; }
    public string? Language { get; set; }
    public string? Timezone { get; set; }
    public string? NotificationSettings { get; set; }
    public string? PrivacySettings { get; set; }
}
