# Profile Management Models

## Core Aggregate

### ProfileAggregate

Located in: `StartupStarter.Core\Model\ProfileAggregate\`

#### Folder Structure
```
ProfileAggregate/
├── Entities/
│   ├── Profile.cs
│   ├── ProfilePreferences.cs
│   └── ProfileShare.cs
├── Enums/
│   ├── ProfileType.cs
│   └── PermissionLevel.cs
└── Events/
    ├── ProfileCreatedEvent.cs
    ├── ProfileUpdatedEvent.cs
    ├── ProfileDeletedEvent.cs
    ├── ProfileAvatarUpdatedEvent.cs
    ├── ProfilePreferencesUpdatedEvent.cs
    ├── ProfileSharedEvent.cs
    ├── ProfileShareRevokedEvent.cs
    ├── ProfileSetAsDefaultEvent.cs
    ├── ProfileDashboardAddedEvent.cs
    └── ProfileDashboardRemovedEvent.cs
```

#### Entities

Located in: `StartupStarter.Core\Model\ProfileAggregate\Entities\`

**Profile.cs** (Aggregate Root)
```csharp
public class Profile
{
    public string ProfileId { get; private set; }
    public string ProfileName { get; private set; }
    public string AccountId { get; private set; }
    public string CreatedBy { get; private set; }
    public ProfileType ProfileType { get; private set; }
    public bool IsDefault { get; private set; }
    public string AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private readonly List<ProfilePreferences> _preferences = new();
    public IReadOnlyCollection<ProfilePreferences> Preferences => _preferences.AsReadOnly();

    private readonly List<ProfileShare> _shares = new();
    public IReadOnlyCollection<ProfileShare> Shares => _shares.AsReadOnly();

    private readonly List<string> _dashboardIds = new();
    public IReadOnlyCollection<string> DashboardIds => _dashboardIds.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Update(Dictionary<string, object> updatedFields, string updatedBy);
    public void Delete(string deletedBy);
    public void UpdateAvatar(string newAvatarUrl, string updatedBy);
    public void UpdatePreferences(string category, Dictionary<string, object> preferences, string updatedBy);
    public void ShareWith(List<string> userIds, PermissionLevel permissionLevel, string sharedBy);
    public void RevokeShare(List<string> userIds, string revokedBy);
    public void SetAsDefault(string userId);
    public void AddDashboard(string dashboardId, string addedBy);
    public void RemoveDashboard(string dashboardId, string removedBy);
}
```

**ProfilePreferences.cs**
```csharp
public class ProfilePreferences
{
    public string ProfilePreferencesId { get; private set; }
    public string ProfileId { get; private set; }
    public string Category { get; private set; }
    public string PreferencesJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Profile Profile { get; private set; }
}
```

**ProfileShare.cs**
```csharp
public class ProfileShare
{
    public string ProfileShareId { get; private set; }
    public string ProfileId { get; private set; }
    public string OwnerUserId { get; private set; }
    public string SharedWithUserId { get; private set; }
    public PermissionLevel PermissionLevel { get; private set; }
    public DateTime SharedAt { get; private set; }

    public Profile Profile { get; private set; }
}
```

#### Enums

Located in: `StartupStarter.Core\Model\ProfileAggregate\Enums\`

**ProfileType.cs**
```csharp
public enum ProfileType
{
    Personal,
    Project,
    Department,
    Team
}
```

**PermissionLevel.cs**
```csharp
public enum PermissionLevel
{
    View,
    Edit,
    Admin
}
```

#### Domain Events

Located in: `StartupStarter.Core\Model\ProfileAggregate\Events\`

All profile events go here (ProfileCreatedEvent.cs, ProfileUpdatedEvent.cs, etc.)

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\ProfileManagement\Commands\`

**CreateProfileCommand.cs**, **UpdateProfileCommand.cs**, **DeleteProfileCommand.cs**, **UpdateProfileAvatarCommand.cs**, **UpdateProfilePreferencesCommand.cs**, **ShareProfileCommand.cs**, **SetDefaultProfileCommand.cs**
