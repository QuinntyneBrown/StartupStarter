using StartupStarter.Core.Model.ProfileAggregate.Enums;
using StartupStarter.Core.Model.ProfileAggregate.Events;

namespace StartupStarter.Core.Model.ProfileAggregate.Entities;

public class Profile
{
    public string ProfileId { get; private set; }
    public string ProfileName { get; private set; }
    public string AccountId { get; private set; }
    public string CreatedBy { get; private set; }
    public ProfileType ProfileType { get; private set; }
    public bool IsDefault { get; private set; }
    public string? AvatarUrl { get; private set; }
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

    private Profile() { }

    public Profile(string profileId, string profileName, string accountId,
        string createdBy, ProfileType profileType, bool isDefault)
    {
        ProfileId = profileId;
        ProfileName = profileName;
        AccountId = accountId;
        CreatedBy = createdBy;
        ProfileType = profileType;
        IsDefault = isDefault;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProfileCreatedEvent
        {
            ProfileId = ProfileId,
            ProfileName = ProfileName,
            AccountId = AccountId,
            CreatedBy = CreatedBy,
            ProfileType = ProfileType,
            IsDefault = IsDefault,
            Timestamp = CreatedAt
        });
    }

    public void Update(Dictionary<string, object> updatedFields, string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProfileUpdatedEvent
        {
            ProfileId = ProfileId,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            PreviousValues = new Dictionary<string, object>(),
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProfileDeletedEvent
        {
            ProfileId = ProfileId,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            DashboardCount = _dashboardIds.Count,
            Timestamp = UpdatedAt.Value
        });
    }

    public void UpdateAvatar(string newAvatarUrl, string updatedBy)
    {
        var previousUrl = AvatarUrl;
        AvatarUrl = newAvatarUrl;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProfileAvatarUpdatedEvent
        {
            ProfileId = ProfileId,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            PreviousAvatarUrl = previousUrl,
            NewAvatarUrl = newAvatarUrl,
            Timestamp = UpdatedAt.Value
        });
    }

    public void SetAsDefault(string userId)
    {
        IsDefault = true;
        AddDomainEvent(new ProfileSetAsDefaultEvent
        {
            ProfileId = ProfileId,
            AccountId = AccountId,
            UserId = userId,
            PreviousDefaultProfileId = null,
            Timestamp = DateTime.UtcNow
        });
    }

    public void AddDashboard(string dashboardId, string addedBy)
    {
        if (!_dashboardIds.Contains(dashboardId))
        {
            _dashboardIds.Add(dashboardId);
            AddDomainEvent(new ProfileDashboardAddedEvent
            {
                ProfileId = ProfileId,
                AccountId = AccountId,
                DashboardId = dashboardId,
                AddedBy = addedBy,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    public void RemoveDashboard(string dashboardId, string removedBy)
    {
        if (_dashboardIds.Remove(dashboardId))
        {
            AddDomainEvent(new ProfileDashboardRemovedEvent
            {
                ProfileId = ProfileId,
                AccountId = AccountId,
                DashboardId = dashboardId,
                RemovedBy = removedBy,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
