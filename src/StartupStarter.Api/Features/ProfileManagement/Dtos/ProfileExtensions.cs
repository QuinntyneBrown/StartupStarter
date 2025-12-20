using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Api.Features.ProfileManagement.Dtos;

public static class ProfileExtensions
{
    public static ProfileDto ToDto(this Profile profile)
    {
        return new ProfileDto
        {
            ProfileId = profile.ProfileId,
            UserId = profile.CreatedBy,
            DisplayName = profile.ProfileName,
            Bio = null,
            AvatarUrl = profile.AvatarUrl,
            IsDefault = profile.IsDefault,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            DeletedAt = profile.DeletedAt
        };
    }

    public static ProfilePreferencesDto ToDto(this ProfilePreferences preferences)
    {
        return new ProfilePreferencesDto
        {
            PreferencesId = preferences.ProfilePreferencesId,
            ProfileId = preferences.ProfileId,
            Theme = null,
            Language = null,
            Timezone = null,
            NotificationSettings = null,
            PrivacySettings = null
        };
    }

    public static ProfileShareDto ToDto(this ProfileShare share)
    {
        return new ProfileShareDto
        {
            ShareId = share.ProfileShareId,
            ProfileId = share.ProfileId,
            SharedWithUserId = share.SharedWithUserId,
            PermissionLevel = share.PermissionLevel.ToString(),
            CreatedAt = share.SharedAt
        };
    }
}
