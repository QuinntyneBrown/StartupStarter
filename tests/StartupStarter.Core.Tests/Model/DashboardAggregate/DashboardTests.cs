using FluentAssertions;
using StartupStarter.Core.Model.DashboardAggregate.Entities;
using StartupStarter.Core.Model.DashboardAggregate.Enums;
using StartupStarter.Core.Model.DashboardAggregate.Events;
using StartupStarter.Core.Model.DashboardAggregate.ValueObjects;

namespace StartupStarter.Core.Tests.Model.DashboardAggregate;

public class DashboardTests
{
    private const string ValidDashboardId = "dashboard-123";
    private const string ValidDashboardName = "Main Dashboard";
    private const string ValidProfileId = "profile-456";
    private const string ValidAccountId = "acc-789";
    private const string ValidCreatedBy = "admin";
    private const string ValidTemplate = "default";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidData_ShouldCreateDashboard()
    {
        // Act
        var dashboard = new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            true,
            ValidTemplate,
            LayoutType.Grid);

        // Assert
        dashboard.DashboardId.Should().Be(ValidDashboardId);
        dashboard.DashboardName.Should().Be(ValidDashboardName);
        dashboard.ProfileId.Should().Be(ValidProfileId);
        dashboard.AccountId.Should().Be(ValidAccountId);
        dashboard.CreatedBy.Should().Be(ValidCreatedBy);
        dashboard.IsDefault.Should().BeTrue();
        dashboard.Template.Should().Be(ValidTemplate);
        dashboard.LayoutType.Should().Be(LayoutType.Grid);
        dashboard.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        dashboard.UpdatedAt.Should().BeNull();
        dashboard.DeletedAt.Should().BeNull();
        dashboard.Cards.Should().BeEmpty();
        dashboard.Shares.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldRaiseDashboardCreatedEvent()
    {
        // Act
        var dashboard = new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            false,
            ValidTemplate,
            LayoutType.Freeform);

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(ValidDashboardId);
        domainEvent.DashboardName.Should().Be(ValidDashboardName);
        domainEvent.ProfileId.Should().Be(ValidProfileId);
        domainEvent.AccountId.Should().Be(ValidAccountId);
        domainEvent.CreatedBy.Should().Be(ValidCreatedBy);
        domainEvent.IsDefault.Should().BeFalse();
        domainEvent.LayoutType.Should().Be(LayoutType.Freeform.ToString());
    }

    [Fact]
    public void Constructor_WithNullTemplate_ShouldUseEmptyString()
    {
        // Act
        var dashboard = new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            false,
            null!,
            LayoutType.Grid);

        // Assert
        dashboard.Template.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyDashboardId_ShouldThrowArgumentException(string? dashboardId)
    {
        // Act & Assert
        var act = () => new Dashboard(
            dashboardId!,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            false,
            ValidTemplate,
            LayoutType.Grid);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("dashboardId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyDashboardName_ShouldThrowArgumentException(string? dashboardName)
    {
        // Act & Assert
        var act = () => new Dashboard(
            ValidDashboardId,
            dashboardName!,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            false,
            ValidTemplate,
            LayoutType.Grid);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("dashboardName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyProfileId_ShouldThrowArgumentException(string? profileId)
    {
        // Act & Assert
        var act = () => new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            profileId!,
            ValidAccountId,
            ValidCreatedBy,
            false,
            ValidTemplate,
            LayoutType.Grid);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("profileId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        // Act & Assert
        var act = () => new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            accountId!,
            ValidCreatedBy,
            false,
            ValidTemplate,
            LayoutType.Grid);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("accountId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyCreatedBy_ShouldThrowArgumentException(string? createdBy)
    {
        // Act & Assert
        var act = () => new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            createdBy!,
            false,
            ValidTemplate,
            LayoutType.Grid);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("createdBy");
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_WithValidData_ShouldUpdateFields()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "DashboardName", "Updated Dashboard" },
            { "Template", "new-template" }
        };

        // Act
        dashboard.Update(updatedFields, "admin");

        // Assert
        dashboard.DashboardName.Should().Be("Updated Dashboard");
        dashboard.Template.Should().Be("new-template");
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WithValidData_ShouldRaiseDashboardUpdatedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var originalName = dashboard.DashboardName;
        dashboard.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "DashboardName", "New Name" }
        };

        // Act
        dashboard.Update(updatedFields, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.UpdatedBy.Should().Be("admin");
        domainEvent.PreviousValues.Should().ContainKey("DashboardName");
        domainEvent.PreviousValues["DashboardName"].Should().Be(originalName);
    }

    [Fact]
    public void Update_WithNullUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.Update(null!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Fact]
    public void Update_WithEmptyUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.Update(new Dictionary<string, object>(), "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithEmptyUpdatedBy_ShouldThrowArgumentException(string? updatedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();
        var updatedFields = new Dictionary<string, object> { { "DashboardName", "New" } };

        // Act & Assert
        var act = () => dashboard.Update(updatedFields, updatedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedBy");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_Dashboard_ShouldSetDeletedAt()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();

        // Act
        dashboard.Delete("admin");

        // Assert
        dashboard.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_Dashboard_ShouldRaiseDashboardDeletedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();

        // Act
        dashboard.Delete("admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardDeletedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.DashboardName.Should().Be(dashboard.DashboardName);
        domainEvent.DeletedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Delete_WithEmptyDeletedBy_ShouldThrowArgumentException(string? deletedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.Delete(deletedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("deletedBy");
    }

    #endregion

    #region Clone Tests

    [Fact]
    public void Clone_Dashboard_ShouldCreateNewDashboardWithCopyName()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var newDashboardId = "new-dashboard-456";

        // Act
        var clonedDashboard = dashboard.Clone(newDashboardId, "admin");

        // Assert
        clonedDashboard.DashboardId.Should().Be(newDashboardId);
        clonedDashboard.DashboardName.Should().Be($"{dashboard.DashboardName} (Copy)");
        clonedDashboard.ProfileId.Should().Be(dashboard.ProfileId);
        clonedDashboard.AccountId.Should().Be(dashboard.AccountId);
        clonedDashboard.IsDefault.Should().BeFalse();
        clonedDashboard.Template.Should().Be(dashboard.Template);
        clonedDashboard.LayoutType.Should().Be(dashboard.LayoutType);
    }

    [Fact]
    public void Clone_Dashboard_ShouldRaiseDashboardClonedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var newDashboardId = "new-dashboard-456";

        // Act
        dashboard.Clone(newDashboardId, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardClonedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.OriginalDashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.NewDashboardId.Should().Be(newDashboardId);
        domainEvent.ClonedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Clone_WithEmptyNewDashboardId_ShouldThrowArgumentException(string? newDashboardId)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.Clone(newDashboardId!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("newDashboardId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Clone_WithEmptyClonedBy_ShouldThrowArgumentException(string? clonedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.Clone("new-id", clonedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("clonedBy");
    }

    #endregion

    #region SetAsDefault Tests

    [Fact]
    public void SetAsDefault_Dashboard_ShouldSetIsDefaultToTrue()
    {
        // Arrange
        var dashboard = new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            false,
            ValidTemplate,
            LayoutType.Grid);
        dashboard.ClearDomainEvents();

        // Act
        dashboard.SetAsDefault("admin");

        // Assert
        dashboard.IsDefault.Should().BeTrue();
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetAsDefault_Dashboard_ShouldRaiseDashboardSetAsDefaultEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();

        // Act
        dashboard.SetAsDefault("admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardSetAsDefaultEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.ProfileId.Should().Be(dashboard.ProfileId);
        domainEvent.SetBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void SetAsDefault_WithEmptySetBy_ShouldThrowArgumentException(string? setBy)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.SetAsDefault(setBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("setBy");
    }

    #endregion

    #region ShareWith Tests

    [Fact]
    public void ShareWith_ValidUserIds_ShouldRaiseDashboardSharedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var userIds = new List<string> { "user-1", "user-2", "user-3" };

        // Act
        dashboard.ShareWith(userIds, PermissionLevel.ReadWrite);

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardSharedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.SharedWithUserIds.Should().BeEquivalentTo(userIds);
        domainEvent.PermissionLevel.Should().Be(PermissionLevel.ReadWrite.ToString());
    }

    [Fact]
    public void ShareWith_NullUserIds_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.ShareWith(null!, PermissionLevel.View);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("userIds");
    }

    [Fact]
    public void ShareWith_EmptyUserIds_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.ShareWith(new List<string>(), PermissionLevel.View);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("userIds");
    }

    #endregion

    #region RevokeShare Tests

    [Fact]
    public void RevokeShare_ValidUserIds_ShouldRaiseDashboardShareRevokedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var userIds = new List<string> { "user-1", "user-2" };

        // Act
        dashboard.RevokeShare(userIds);

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardShareRevokedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.RevokedUserIds.Should().BeEquivalentTo(userIds);
    }

    [Fact]
    public void RevokeShare_NullUserIds_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.RevokeShare(null!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("userIds");
    }

    [Fact]
    public void RevokeShare_EmptyUserIds_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.RevokeShare(new List<string>());
        act.Should().Throw<ArgumentException>()
            .WithParameterName("userIds");
    }

    #endregion

    #region AddCard Tests

    [Fact]
    public void AddCard_ValidCard_ShouldAddToCollection()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var card = CreateCard();

        // Act
        dashboard.AddCard(card, "admin");

        // Assert
        dashboard.Cards.Should().ContainSingle();
        dashboard.Cards.First().Should().Be(card);
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddCard_ValidCard_ShouldRaiseDashboardCardAddedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var card = CreateCard();

        // Act
        dashboard.AddCard(card, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardCardAddedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.DashboardId.Should().Be(dashboard.DashboardId);
        domainEvent.CardId.Should().Be(card.CardId);
        domainEvent.CardType.Should().Be(card.CardType);
        domainEvent.AddedBy.Should().Be("admin");
    }

    [Fact]
    public void AddCard_NullCard_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.AddCard(null!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("card");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AddCard_WithEmptyAddedBy_ShouldThrowArgumentException(string? addedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();
        var card = CreateCard();

        // Act & Assert
        var act = () => dashboard.AddCard(card, addedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("addedBy");
    }

    #endregion

    #region UpdateCard Tests

    [Fact]
    public void UpdateCard_WithValidData_ShouldRaiseDashboardCardUpdatedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "Title", "Updated Card Title" }
        };

        // Act
        dashboard.UpdateCard("card-123", updatedFields, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardCardUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.CardId.Should().Be("card-123");
        domainEvent.UpdatedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateCard_WithEmptyCardId_ShouldThrowArgumentException(string? cardId)
    {
        // Arrange
        var dashboard = CreateDashboard();
        var updatedFields = new Dictionary<string, object> { { "Title", "New" } };

        // Act & Assert
        var act = () => dashboard.UpdateCard(cardId!, updatedFields, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("cardId");
    }

    [Fact]
    public void UpdateCard_WithNullUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.UpdateCard("card-123", null!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Fact]
    public void UpdateCard_WithEmptyUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.UpdateCard("card-123", new Dictionary<string, object>(), "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateCard_WithEmptyUpdatedBy_ShouldThrowArgumentException(string? updatedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();
        var updatedFields = new Dictionary<string, object> { { "Title", "New" } };

        // Act & Assert
        var act = () => dashboard.UpdateCard("card-123", updatedFields, updatedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedBy");
    }

    #endregion

    #region RemoveCard Tests

    [Fact]
    public void RemoveCard_ExistingCard_ShouldRemoveFromCollection()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var card = CreateCard();
        dashboard.AddCard(card, "admin");
        dashboard.ClearDomainEvents();

        // Act
        dashboard.RemoveCard(card.CardId, "admin");

        // Assert
        dashboard.Cards.Should().BeEmpty();
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoveCard_ExistingCard_ShouldRaiseDashboardCardRemovedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var card = CreateCard();
        dashboard.AddCard(card, "admin");
        dashboard.ClearDomainEvents();

        // Act
        dashboard.RemoveCard(card.CardId, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardCardRemovedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.CardId.Should().Be(card.CardId);
        domainEvent.RemovedBy.Should().Be("admin");
    }

    [Fact]
    public void RemoveCard_NonExistingCard_ShouldStillRaiseEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();

        // Act
        dashboard.RemoveCard("non-existing-card", "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RemoveCard_WithEmptyCardId_ShouldThrowArgumentException(string? cardId)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.RemoveCard(cardId!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("cardId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RemoveCard_WithEmptyRemovedBy_ShouldThrowArgumentException(string? removedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.RemoveCard("card-123", removedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("removedBy");
    }

    #endregion

    #region RepositionCard Tests

    [Fact]
    public void RepositionCard_ExistingCard_ShouldUpdatePosition()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var card = CreateCard();
        dashboard.AddCard(card, "admin");
        dashboard.ClearDomainEvents();
        var newPosition = new CardPosition(5, 5, 3, 3);

        // Act
        dashboard.RepositionCard(card.CardId, newPosition, "admin");

        // Assert
        var updatedCard = dashboard.Cards.First();
        updatedCard.Position.Row.Should().Be(5);
        updatedCard.Position.Column.Should().Be(5);
    }

    [Fact]
    public void RepositionCard_ExistingCard_ShouldRaiseDashboardCardRepositionedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var card = CreateCard();
        dashboard.AddCard(card, "admin");
        dashboard.ClearDomainEvents();
        var newPosition = new CardPosition(2, 3, 2, 2);

        // Act
        dashboard.RepositionCard(card.CardId, newPosition, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardCardRepositionedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.CardId.Should().Be(card.CardId);
        domainEvent.NewPosition.Should().Be(newPosition);
        domainEvent.RepositionedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RepositionCard_WithEmptyCardId_ShouldThrowArgumentException(string? cardId)
    {
        // Arrange
        var dashboard = CreateDashboard();
        var position = new CardPosition(1, 1, 1, 1);

        // Act & Assert
        var act = () => dashboard.RepositionCard(cardId!, position, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("cardId");
    }

    [Fact]
    public void RepositionCard_WithNullPosition_ShouldThrowArgumentException()
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.RepositionCard("card-123", null!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("newPosition");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RepositionCard_WithEmptyRepositionedBy_ShouldThrowArgumentException(string? repositionedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();
        var position = new CardPosition(1, 1, 1, 1);

        // Act & Assert
        var act = () => dashboard.RepositionCard("card-123", position, repositionedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("repositionedBy");
    }

    #endregion

    #region ChangeLayout Tests

    [Fact]
    public void ChangeLayout_WithValidData_ShouldUpdateLayoutType()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();

        // Act
        dashboard.ChangeLayout(LayoutType.Freeform, "admin");

        // Assert
        dashboard.LayoutType.Should().Be(LayoutType.Freeform);
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangeLayout_WithValidData_ShouldRaiseDashboardLayoutChangedEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var previousLayout = dashboard.LayoutType;
        dashboard.ClearDomainEvents();

        // Act
        dashboard.ChangeLayout(LayoutType.Freeform, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardLayoutChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.PreviousLayout.Should().Be(previousLayout.ToString());
        domainEvent.NewLayout.Should().Be(LayoutType.Freeform.ToString());
        domainEvent.ChangedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ChangeLayout_WithEmptyChangedBy_ShouldThrowArgumentException(string? changedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.ChangeLayout(LayoutType.Freeform, changedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("changedBy");
    }

    #endregion

    #region MoveToProfile Tests

    [Fact]
    public void MoveToProfile_WithValidData_ShouldUpdateProfileId()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.ClearDomainEvents();
        var newProfileId = "new-profile-123";

        // Act
        dashboard.MoveToProfile(newProfileId, "admin");

        // Assert
        dashboard.ProfileId.Should().Be(newProfileId);
        dashboard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MoveToProfile_WithValidData_ShouldRaiseDashboardMovedToProfileEvent()
    {
        // Arrange
        var dashboard = CreateDashboard();
        var previousProfileId = dashboard.ProfileId;
        dashboard.ClearDomainEvents();
        var newProfileId = "new-profile-123";

        // Act
        dashboard.MoveToProfile(newProfileId, "admin");

        // Assert
        dashboard.DomainEvents.Should().ContainSingle();
        var domainEvent = dashboard.DomainEvents.First() as DashboardMovedToProfileEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.PreviousProfileId.Should().Be(previousProfileId);
        domainEvent.NewProfileId.Should().Be(newProfileId);
        domainEvent.MovedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void MoveToProfile_WithEmptyNewProfileId_ShouldThrowArgumentException(string? newProfileId)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.MoveToProfile(newProfileId!, "admin");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("newProfileId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void MoveToProfile_WithEmptyMovedBy_ShouldThrowArgumentException(string? movedBy)
    {
        // Arrange
        var dashboard = CreateDashboard();

        // Act & Assert
        var act = () => dashboard.MoveToProfile("new-profile", movedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("movedBy");
    }

    #endregion

    #region ClearDomainEvents Tests

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var dashboard = CreateDashboard();
        dashboard.DomainEvents.Should().NotBeEmpty();

        // Act
        dashboard.ClearDomainEvents();

        // Assert
        dashboard.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static Dashboard CreateDashboard()
    {
        return new Dashboard(
            ValidDashboardId,
            ValidDashboardName,
            ValidProfileId,
            ValidAccountId,
            ValidCreatedBy,
            true,
            ValidTemplate,
            LayoutType.Grid);
    }

    private static DashboardCard CreateCard()
    {
        return new DashboardCard(
            "card-123",
            ValidDashboardId,
            "chart",
            "{}",
            new CardPosition(0, 0, 2, 2));
    }

    #endregion
}
