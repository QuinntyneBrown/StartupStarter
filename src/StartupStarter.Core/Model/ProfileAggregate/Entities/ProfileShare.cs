using StartupStarter.Core.Model.ProfileAggregate.Enums;

namespace StartupStarter.Core.Model.ProfileAggregate.Entities;

public class ProfileShare
{
    public string ProfileShareId { get; private set; }
    public string ProfileId { get; private set; }
    public string OwnerUserId { get; private set; }
    public string SharedWithUserId { get; private set; }
    public PermissionLevel PermissionLevel { get; private set; }
    public DateTime SharedAt { get; private set; }

    public Profile Profile { get; private set; } = null!;

    private ProfileShare() { }

    public ProfileShare(string profileShareId, string profileId, string ownerUserId, 
        string sharedWithUserId, PermissionLevel permissionLevel)
    {
        ProfileShareId = profileShareId;
        ProfileId = profileId;
        OwnerUserId = ownerUserId;
        SharedWithUserId = sharedWithUserId;
        PermissionLevel = permissionLevel;
        SharedAt = DateTime.UtcNow;
    }
}
