using FluentAssertions;
using StartupStarter.Core.Model.ContentAggregate.Entities;
using StartupStarter.Core.Model.ContentAggregate.Enums;
using StartupStarter.Core.Model.ContentAggregate.Events;

namespace StartupStarter.Core.Tests.Model.ContentAggregate;

public class ContentTests
{
    private const string ValidContentId = "content-123";
    private const string ValidContentType = "article";
    private const string ValidTitle = "Test Article";
    private const string ValidBody = "This is the content body.";
    private const string ValidAuthorId = "author-456";
    private const string ValidAccountId = "acc-789";
    private const string ValidProfileId = "profile-012";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidData_ShouldCreateContentWithDraftStatus()
    {
        // Act
        var content = new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);

        // Assert
        content.ContentId.Should().Be(ValidContentId);
        content.ContentType.Should().Be(ValidContentType);
        content.Title.Should().Be(ValidTitle);
        content.Body.Should().Be(ValidBody);
        content.AuthorId.Should().Be(ValidAuthorId);
        content.AccountId.Should().Be(ValidAccountId);
        content.ProfileId.Should().Be(ValidProfileId);
        content.Status.Should().Be(ContentStatus.Draft);
        content.CurrentVersion.Should().Be(1);
        content.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        content.PublishedAt.Should().BeNull();
        content.ScheduledPublishDate.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldRaiseContentCreatedEvent()
    {
        // Act
        var content = new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(ValidContentId);
        domainEvent.ContentType.Should().Be(ValidContentType);
        domainEvent.Title.Should().Be(ValidTitle);
        domainEvent.AuthorId.Should().Be(ValidAuthorId);
        domainEvent.Status.Should().Be(ContentStatus.Draft);
    }

    [Fact]
    public void Constructor_WithNullContentType_ShouldUseEmptyString()
    {
        // Act
        var content = new Content(
            ValidContentId,
            null!,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);

        // Assert
        content.ContentType.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithNullBody_ShouldUseEmptyString()
    {
        // Act
        var content = new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            null!,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);

        // Assert
        content.Body.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyContentId_ShouldThrowArgumentException(string? contentId)
    {
        // Act & Assert
        var act = () => new Content(
            contentId!,
            ValidContentType,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("contentId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTitle_ShouldThrowArgumentException(string? title)
    {
        // Act & Assert
        var act = () => new Content(
            ValidContentId,
            ValidContentType,
            title!,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAuthorId_ShouldThrowArgumentException(string? authorId)
    {
        // Act & Assert
        var act = () => new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            ValidBody,
            authorId!,
            ValidAccountId,
            ValidProfileId);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("authorId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyAccountId_ShouldThrowArgumentException(string? accountId)
    {
        // Act & Assert
        var act = () => new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            accountId!,
            ValidProfileId);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("accountId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyProfileId_ShouldThrowArgumentException(string? profileId)
    {
        // Act & Assert
        var act = () => new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            profileId!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("profileId");
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_WithValidData_ShouldUpdateFieldsAndIncrementVersion()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();
        var initialVersion = content.CurrentVersion;
        var updatedFields = new Dictionary<string, object>
        {
            { "Title", "Updated Title" },
            { "Body", "Updated body content" }
        };

        // Act
        content.Update(updatedFields, "editor");

        // Assert
        content.Title.Should().Be("Updated Title");
        content.Body.Should().Be("Updated body content");
        content.CurrentVersion.Should().Be(initialVersion + 1);
        content.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WithValidData_ShouldRaiseContentUpdatedEvent()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();
        var updatedFields = new Dictionary<string, object>
        {
            { "Title", "New Title" }
        };

        // Act
        content.Update(updatedFields, "editor");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentUpdatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.UpdatedBy.Should().Be("editor");
        domainEvent.VersionNumber.Should().Be(content.CurrentVersion);
    }

    [Fact]
    public void Update_WithNullUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.Update(null!, "editor");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedFields");
    }

    [Fact]
    public void Update_WithEmptyUpdatedFields_ShouldThrowArgumentException()
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.Update(new Dictionary<string, object>(), "editor");
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
        var content = CreateContent();
        var updatedFields = new Dictionary<string, object> { { "Title", "New" } };

        // Act & Assert
        var act = () => content.Update(updatedFields, updatedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("updatedBy");
    }

    #endregion

    #region Publish Tests

    [Fact]
    public void Publish_ImmediatePublish_ShouldChangeStatusToPublished()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();

        // Act
        content.Publish("editor");

        // Assert
        content.Status.Should().Be(ContentStatus.Published);
        content.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Publish_ImmediatePublish_ShouldRaiseContentPublishedEvent()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();

        // Act
        content.Publish("editor");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentPublishedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.PublishedBy.Should().Be("editor");
        domainEvent.ScheduledPublish.Should().BeFalse();
    }

    [Fact]
    public void Publish_ScheduledPublish_ShouldNotChangeStatusImmediately()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act
        content.Publish("editor", futureDate);

        // Assert
        content.Status.Should().Be(ContentStatus.Draft);
        content.PublishedAt.Should().BeNull();
    }

    [Fact]
    public void Publish_ScheduledPublish_ShouldRaiseEventWithScheduledFlag()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act
        content.Publish("editor", futureDate);

        // Assert
        var domainEvent = content.DomainEvents.First() as ContentPublishedEvent;
        domainEvent!.ScheduledPublish.Should().BeTrue();
        domainEvent.PublishDate.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Publish_WithEmptyPublishedBy_ShouldThrowArgumentException(string? publishedBy)
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.Publish(publishedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("publishedBy");
    }

    #endregion

    #region Unpublish Tests

    [Fact]
    public void Unpublish_PublishedContent_ShouldChangeStatusToUnpublished()
    {
        // Arrange
        var content = CreatePublishedContent();
        content.ClearDomainEvents();

        // Act
        content.Unpublish("editor", "Content review required");

        // Assert
        content.Status.Should().Be(ContentStatus.Unpublished);
        content.UnpublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Unpublish_PublishedContent_ShouldRaiseContentUnpublishedEvent()
    {
        // Arrange
        var content = CreatePublishedContent();
        content.ClearDomainEvents();

        // Act
        content.Unpublish("editor", "Outdated information");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentUnpublishedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.UnpublishedBy.Should().Be("editor");
        domainEvent.Reason.Should().Be("Outdated information");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Unpublish_WithEmptyUnpublishedBy_ShouldThrowArgumentException(string? unpublishedBy)
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.Unpublish(unpublishedBy!, "reason");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("unpublishedBy");
    }

    #endregion

    #region ChangeStatus Tests

    [Fact]
    public void ChangeStatus_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();

        // Act
        content.ChangeStatus(ContentStatus.Archived, "admin");

        // Assert
        content.Status.Should().Be(ContentStatus.Archived);
        content.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangeStatus_WithValidData_ShouldRaiseContentStatusChangedEvent()
    {
        // Arrange
        var content = CreateContent();
        var previousStatus = content.Status;
        content.ClearDomainEvents();

        // Act
        content.ChangeStatus(ContentStatus.Review, "reviewer");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentStatusChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.PreviousStatus.Should().Be(previousStatus.ToString());
        domainEvent.NewStatus.Should().Be(ContentStatus.Review.ToString());
        domainEvent.ChangedBy.Should().Be("reviewer");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ChangeStatus_WithEmptyChangedBy_ShouldThrowArgumentException(string? changedBy)
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.ChangeStatus(ContentStatus.Review, changedBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("changedBy");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_Content_ShouldChangeStatusToDeleted()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();

        // Act
        content.Delete("admin", DeletionType.Soft);

        // Assert
        content.Status.Should().Be(ContentStatus.Deleted);
        content.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        content.DeletionType.Should().Be(DeletionType.Soft);
    }

    [Fact]
    public void Delete_Content_ShouldRaiseContentDeletedEvent()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();

        // Act
        content.Delete("admin", DeletionType.Hard);

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentDeletedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.DeletedBy.Should().Be("admin");
        domainEvent.DeletionType.Should().Be(DeletionType.Hard);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Delete_WithEmptyDeletedBy_ShouldThrowArgumentException(string? deletedBy)
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.Delete(deletedBy!, DeletionType.Soft);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("deletedBy");
    }

    #endregion

    #region CreateVersion Tests

    [Fact]
    public void CreateVersion_ShouldIncrementVersionNumber()
    {
        // Arrange
        var content = CreateContent();
        var initialVersion = content.CurrentVersion;
        content.ClearDomainEvents();

        // Act
        content.CreateVersion("editor", "Major update");

        // Assert
        content.CurrentVersion.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void CreateVersion_ShouldRaiseContentVersionCreatedEvent()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();

        // Act
        content.CreateVersion("editor", "Added new section");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentVersionCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.CreatedBy.Should().Be("editor");
        domainEvent.ChangeDescription.Should().Be("Added new section");
        domainEvent.VersionNumber.Should().Be(content.CurrentVersion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateVersion_WithEmptyCreatedBy_ShouldThrowArgumentException(string? createdBy)
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.CreateVersion(createdBy!, "description");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("createdBy");
    }

    #endregion

    #region RestoreVersion Tests

    [Fact]
    public void RestoreVersion_WithValidVersion_ShouldIncrementVersionNumber()
    {
        // Arrange
        var content = CreateContent();
        content.CreateVersion("editor", "v2");
        content.CreateVersion("editor", "v3");
        var currentVersion = content.CurrentVersion;
        content.ClearDomainEvents();

        // Act
        content.RestoreVersion(1, "editor");

        // Assert
        content.CurrentVersion.Should().Be(currentVersion + 1);
    }

    [Fact]
    public void RestoreVersion_WithValidVersion_ShouldRaiseContentVersionRestoredEvent()
    {
        // Arrange
        var content = CreateContent();
        content.CreateVersion("editor", "v2");
        content.ClearDomainEvents();

        // Act
        content.RestoreVersion(1, "editor");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentVersionRestoredEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.RestoredVersionNumber.Should().Be(1);
        domainEvent.RestoredBy.Should().Be("editor");
    }

    [Fact]
    public void RestoreVersion_WithInvalidVersion_ShouldThrowArgumentException()
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.RestoreVersion(0, "editor");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("versionNumber");
    }

    [Fact]
    public void RestoreVersion_WithVersionGreaterThanCurrent_ShouldThrowArgumentException()
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.RestoreVersion(10, "editor");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("versionNumber");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RestoreVersion_WithEmptyRestoredBy_ShouldThrowArgumentException(string? restoredBy)
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.RestoreVersion(1, restoredBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("restoredBy");
    }

    #endregion

    #region Schedule Tests

    [Fact]
    public void Schedule_WithFutureDate_ShouldSetScheduledPublishDate()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act
        content.Schedule(futureDate, "scheduler");

        // Assert
        content.ScheduledPublishDate.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
        content.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Schedule_WithFutureDate_ShouldRaiseContentScheduledEvent()
    {
        // Arrange
        var content = CreateContent();
        content.ClearDomainEvents();
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act
        content.Schedule(futureDate, "scheduler");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentScheduledEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.ScheduledPublishDate.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
        domainEvent.ScheduledBy.Should().Be("scheduler");
    }

    [Fact]
    public void Schedule_WithPastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var content = CreateContent();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var act = () => content.Schedule(pastDate, "scheduler");
        act.Should().Throw<ArgumentException>()
            .WithParameterName("publishDate");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Schedule_WithEmptyScheduledBy_ShouldThrowArgumentException(string? scheduledBy)
    {
        // Arrange
        var content = CreateContent();
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act & Assert
        var act = () => content.Schedule(futureDate, scheduledBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("scheduledBy");
    }

    #endregion

    #region CancelSchedule Tests

    [Fact]
    public void CancelSchedule_WithScheduledContent_ShouldClearScheduledDate()
    {
        // Arrange
        var content = CreateScheduledContent();
        content.ClearDomainEvents();

        // Act
        content.CancelSchedule("scheduler");

        // Assert
        content.ScheduledPublishDate.Should().BeNull();
        content.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CancelSchedule_WithScheduledContent_ShouldRaiseContentScheduleCancelledEvent()
    {
        // Arrange
        var content = CreateScheduledContent();
        var originalDate = content.ScheduledPublishDate!.Value;
        content.ClearDomainEvents();

        // Act
        content.CancelSchedule("scheduler");

        // Assert
        content.DomainEvents.Should().ContainSingle();
        var domainEvent = content.DomainEvents.First() as ContentScheduleCancelledEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.ContentId.Should().Be(content.ContentId);
        domainEvent.CancelledBy.Should().Be("scheduler");
        domainEvent.OriginalScheduledDate.Should().BeCloseTo(originalDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CancelSchedule_WithNoSchedule_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var content = CreateContent();

        // Act & Assert
        var act = () => content.CancelSchedule("scheduler");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No scheduled publish date to cancel");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CancelSchedule_WithEmptyCancelledBy_ShouldThrowArgumentException(string? cancelledBy)
    {
        // Arrange
        var content = CreateScheduledContent();

        // Act & Assert
        var act = () => content.CancelSchedule(cancelledBy!);
        act.Should().Throw<ArgumentException>()
            .WithParameterName("cancelledBy");
    }

    #endregion

    #region ClearDomainEvents Tests

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var content = CreateContent();
        content.DomainEvents.Should().NotBeEmpty();

        // Act
        content.ClearDomainEvents();

        // Assert
        content.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static Content CreateContent()
    {
        return new Content(
            ValidContentId,
            ValidContentType,
            ValidTitle,
            ValidBody,
            ValidAuthorId,
            ValidAccountId,
            ValidProfileId);
    }

    private static Content CreatePublishedContent()
    {
        var content = CreateContent();
        content.Publish("editor");
        return content;
    }

    private static Content CreateScheduledContent()
    {
        var content = CreateContent();
        content.Schedule(DateTime.UtcNow.AddDays(7), "scheduler");
        return content;
    }

    #endregion
}
