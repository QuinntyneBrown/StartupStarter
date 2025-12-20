# Dashboard Management Models

## Core Aggregate

### DashboardAggregate

Located in: `StartupStarter.Core\Model\DashboardAggregate\`

#### Entities

**Dashboard.cs** (Aggregate Root)
```csharp
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

    public void Update(Dictionary<string, object> updatedFields, string updatedBy);
    public void Delete(string deletedBy);
    public Dashboard Clone(string newDashboardId, string clonedBy);
    public void SetAsDefault(string setBy);
    public void ShareWith(List<string> userIds, PermissionLevel permissionLevel);
    public void RevokeShare(List<string> userIds);
    public void AddCard(DashboardCard card, string addedBy);
    public void UpdateCard(string cardId, Dictionary<string, object> updatedFields, string updatedBy);
    public void RemoveCard(string cardId, string removedBy);
    public void RepositionCard(string cardId, CardPosition newPosition, string repositionedBy);
    public void ChangeLayout(LayoutType layoutType, string changedBy);
    public void MoveToProfile(string newProfileId, string movedBy);
}
```

**DashboardCard.cs**
```csharp
public class DashboardCard
{
    public string CardId { get; private set; }
    public string DashboardId { get; private set; }
    public string CardType { get; private set; }
    public string ConfigurationJson { get; private set; }
    public CardPosition Position { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Dashboard Dashboard { get; private set; }

    public void UpdatePosition(CardPosition newPosition);
    public void UpdateConfiguration(Dictionary<string, object> config);
}
```

**DashboardShare.cs**
```csharp
public class DashboardShare
{
    public string DashboardShareId { get; private set; }
    public string DashboardId { get; private set; }
    public string OwnerUserId { get; private set; }
    public string SharedWithUserId { get; private set; }
    public PermissionLevel PermissionLevel { get; private set; }
    public DateTime SharedAt { get; private set; }

    public Dashboard Dashboard { get; private set; }
}
```

#### Value Objects

**CardPosition.cs**
```csharp
public class CardPosition
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public CardPosition(int row, int column, int width, int height)
    {
        Row = row;
        Column = column;
        Width = width;
        Height = height;
    }
}
```

#### Enums

**LayoutType.cs**
```csharp
public enum LayoutType
{
    Grid,
    Masonry,
    Freeform
}
```

**PermissionLevel.cs**
```csharp
public enum PermissionLevel
{
    View,
    Edit
}
```

**ExportFormat.cs**
```csharp
public enum ExportFormat
{
    JSON,
    PDF,
    Image
}
```

#### Domain Events

**DashboardCreatedEvent.cs**, **DashboardUpdatedEvent.cs**, **DashboardDeletedEvent.cs**, **DashboardClonedEvent.cs**, **DashboardSetAsDefaultEvent.cs**, **DashboardSharedEvent.cs**, **DashboardShareRevokedEvent.cs**, **DashboardCardAddedEvent.cs**, **DashboardCardUpdatedEvent.cs**, **DashboardCardRemovedEvent.cs**, **DashboardCardRepositionedEvent.cs**, **DashboardLayoutChangedEvent.cs**, **DashboardExportedEvent.cs**, **DashboardImportedEvent.cs**, **DashboardMovedToProfileEvent.cs**

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\DashboardManagement\Commands\`

**CreateDashboardCommand.cs**
```csharp
public class CreateDashboardCommand : IRequest<DashboardDto>
{
    public string DashboardName { get; set; }
    public string ProfileId { get; set; }
    public string AccountId { get; set; }
    public string CreatedBy { get; set; }
    public bool IsDefault { get; set; }
    public string Template { get; set; }
}
```

**UpdateDashboardCommand.cs**, **DeleteDashboardCommand.cs**, **CloneDashboardCommand.cs**, **SetDefaultDashboardCommand.cs**, **ShareDashboardCommand.cs**, **AddDashboardCardCommand.cs**, **RemoveDashboardCardCommand.cs**, **RepositionDashboardCardCommand.cs**, **ChangeDashboardLayoutCommand.cs**
