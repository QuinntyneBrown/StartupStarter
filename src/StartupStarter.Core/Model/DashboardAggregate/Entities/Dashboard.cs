using StartupStarter.Core.Model.DashboardAggregate.Enums;
using StartupStarter.Core.Model.DashboardAggregate.Events;
using StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

namespace StartupStarter.Core.Model.DashboardAggregate.Entities;

public class Dashboard
{
    public string DashboardId { get; private set; }
    public string DashboardName { get; private set; }
    public string ProfileId { get; private set; }
    public string AccountId { get; private set; }
    public string CreatedBy { get; private set; }
    public bool IsDefault { get; private set; }
    public string Template { get; private set; }
    public LayoutType LayoutType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private readonly List<DashboardCard> _cards = new();
    public IReadOnlyCollection<DashboardCard> Cards => _cards.AsReadOnly();

    private readonly List<DashboardShare> _shares = new();
    public IReadOnlyCollection<DashboardShare> Shares => _shares.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // EF Core constructor
    private Dashboard()
    {
        DashboardId = string.Empty;
        DashboardName = string.Empty;
        ProfileId = string.Empty;
        AccountId = string.Empty;
        CreatedBy = string.Empty;
        Template = string.Empty;
    }

    public Dashboard(string dashboardId, string dashboardName, string profileId,
        string accountId, string createdBy, bool isDefault, string template, LayoutType layoutType)
    {
        if (string.IsNullOrWhiteSpace(dashboardId))
            throw new ArgumentException("Dashboard ID cannot be empty", nameof(dashboardId));
        if (string.IsNullOrWhiteSpace(dashboardName))
            throw new ArgumentException("Dashboard name cannot be empty", nameof(dashboardName));
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Account ID cannot be empty", nameof(accountId));
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be empty", nameof(createdBy));

        DashboardId = dashboardId;
        DashboardName = dashboardName;
        ProfileId = profileId;
        AccountId = accountId;
        CreatedBy = createdBy;
        IsDefault = isDefault;
        Template = template ?? string.Empty;
        LayoutType = layoutType;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardCreatedEvent
        {
            DashboardId = DashboardId,
            DashboardName = DashboardName,
            ProfileId = ProfileId,
            AccountId = AccountId,
            CreatedBy = CreatedBy,
            IsDefault = IsDefault,
            Template = Template,
            LayoutType = LayoutType.ToString(),
            Timestamp = CreatedAt
        });
    }

    public void Update(Dictionary<string, object> updatedFields, string updatedBy)
    {
        if (updatedFields == null || !updatedFields.Any())
            throw new ArgumentException("Updated fields cannot be empty", nameof(updatedFields));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be empty", nameof(updatedBy));

        var previousValues = new Dictionary<string, object>();

        if (updatedFields.ContainsKey("DashboardName"))
        {
            previousValues["DashboardName"] = DashboardName;
            DashboardName = updatedFields["DashboardName"].ToString() ?? DashboardName;
        }

        if (updatedFields.ContainsKey("Template"))
        {
            previousValues["Template"] = Template;
            Template = updatedFields["Template"].ToString() ?? Template;
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardUpdatedEvent
        {
            DashboardId = DashboardId,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            PreviousValues = previousValues,
            Timestamp = UpdatedAt.Value
        });
    }

    public void Delete(string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy cannot be empty", nameof(deletedBy));

        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardDeletedEvent
        {
            DashboardId = DashboardId,
            DashboardName = DashboardName,
            AccountId = AccountId,
            DeletedBy = deletedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public Dashboard Clone(string newDashboardId, string clonedBy)
    {
        if (string.IsNullOrWhiteSpace(newDashboardId))
            throw new ArgumentException("New dashboard ID cannot be empty", nameof(newDashboardId));
        if (string.IsNullOrWhiteSpace(clonedBy))
            throw new ArgumentException("ClonedBy cannot be empty", nameof(clonedBy));

        var clonedDashboard = new Dashboard(
            newDashboardId,
            $"{DashboardName} (Copy)",
            ProfileId,
            AccountId,
            clonedBy,
            false,
            Template,
            LayoutType
        );

        AddDomainEvent(new DashboardClonedEvent
        {
            OriginalDashboardId = DashboardId,
            NewDashboardId = newDashboardId,
            AccountId = AccountId,
            ClonedBy = clonedBy,
            Timestamp = DateTime.UtcNow
        });

        return clonedDashboard;
    }

    public void SetAsDefault(string setBy)
    {
        if (string.IsNullOrWhiteSpace(setBy))
            throw new ArgumentException("SetBy cannot be empty", nameof(setBy));

        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardSetAsDefaultEvent
        {
            DashboardId = DashboardId,
            ProfileId = ProfileId,
            AccountId = AccountId,
            SetBy = setBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ShareWith(List<string> userIds, PermissionLevel permissionLevel)
    {
        if (userIds == null || !userIds.Any())
            throw new ArgumentException("User IDs cannot be empty", nameof(userIds));

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardSharedEvent
        {
            DashboardId = DashboardId,
            AccountId = AccountId,
            SharedWithUserIds = userIds,
            PermissionLevel = permissionLevel.ToString(),
            Timestamp = UpdatedAt.Value
        });
    }

    public void RevokeShare(List<string> userIds)
    {
        if (userIds == null || !userIds.Any())
            throw new ArgumentException("User IDs cannot be empty", nameof(userIds));

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardShareRevokedEvent
        {
            DashboardId = DashboardId,
            AccountId = AccountId,
            RevokedUserIds = userIds,
            Timestamp = UpdatedAt.Value
        });
    }

    public void AddCard(DashboardCard card, string addedBy)
    {
        if (card == null)
            throw new ArgumentException("Card cannot be null", nameof(card));
        if (string.IsNullOrWhiteSpace(addedBy))
            throw new ArgumentException("AddedBy cannot be empty", nameof(addedBy));

        _cards.Add(card);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardCardAddedEvent
        {
            DashboardId = DashboardId,
            CardId = card.CardId,
            CardType = card.CardType,
            AccountId = AccountId,
            AddedBy = addedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void UpdateCard(string cardId, Dictionary<string, object> updatedFields, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be empty", nameof(cardId));
        if (updatedFields == null || !updatedFields.Any())
            throw new ArgumentException("Updated fields cannot be empty", nameof(updatedFields));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be empty", nameof(updatedBy));

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardCardUpdatedEvent
        {
            DashboardId = DashboardId,
            CardId = cardId,
            AccountId = AccountId,
            UpdatedBy = updatedBy,
            UpdatedFields = updatedFields,
            Timestamp = UpdatedAt.Value
        });
    }

    public void RemoveCard(string cardId, string removedBy)
    {
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be empty", nameof(cardId));
        if (string.IsNullOrWhiteSpace(removedBy))
            throw new ArgumentException("RemovedBy cannot be empty", nameof(removedBy));

        var card = _cards.FirstOrDefault(c => c.CardId == cardId);
        if (card != null)
        {
            _cards.Remove(card);
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardCardRemovedEvent
        {
            DashboardId = DashboardId,
            CardId = cardId,
            AccountId = AccountId,
            RemovedBy = removedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void RepositionCard(string cardId, CardPosition newPosition, string repositionedBy)
    {
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be empty", nameof(cardId));
        if (newPosition == null)
            throw new ArgumentException("Position cannot be null", nameof(newPosition));
        if (string.IsNullOrWhiteSpace(repositionedBy))
            throw new ArgumentException("RepositionedBy cannot be empty", nameof(repositionedBy));

        var card = _cards.FirstOrDefault(c => c.CardId == cardId);
        if (card != null)
        {
            card.UpdatePosition(newPosition);
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardCardRepositionedEvent
        {
            DashboardId = DashboardId,
            CardId = cardId,
            AccountId = AccountId,
            NewPosition = newPosition,
            RepositionedBy = repositionedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void ChangeLayout(LayoutType layoutType, string changedBy)
    {
        if (string.IsNullOrWhiteSpace(changedBy))
            throw new ArgumentException("ChangedBy cannot be empty", nameof(changedBy));

        var previousLayout = LayoutType;
        LayoutType = layoutType;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardLayoutChangedEvent
        {
            DashboardId = DashboardId,
            AccountId = AccountId,
            PreviousLayout = previousLayout.ToString(),
            NewLayout = layoutType.ToString(),
            ChangedBy = changedBy,
            Timestamp = UpdatedAt.Value
        });
    }

    public void MoveToProfile(string newProfileId, string movedBy)
    {
        if (string.IsNullOrWhiteSpace(newProfileId))
            throw new ArgumentException("New profile ID cannot be empty", nameof(newProfileId));
        if (string.IsNullOrWhiteSpace(movedBy))
            throw new ArgumentException("MovedBy cannot be empty", nameof(movedBy));

        var previousProfileId = ProfileId;
        ProfileId = newProfileId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DashboardMovedToProfileEvent
        {
            DashboardId = DashboardId,
            AccountId = AccountId,
            PreviousProfileId = previousProfileId,
            NewProfileId = newProfileId,
            MovedBy = movedBy,
            Timestamp = UpdatedAt.Value
        });
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
