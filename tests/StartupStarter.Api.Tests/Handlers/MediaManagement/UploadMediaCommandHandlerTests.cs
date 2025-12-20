using FluentAssertions;
using StartupStarter.Api.Features.MediaManagement.Commands;
using StartupStarter.Api.Tests.Common;

namespace StartupStarter.Api.Tests.Handlers.MediaManagement;

public class UploadMediaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldUploadMedia()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UploadMediaCommandHandler(context);
        var command = new UploadMediaCommand
        {
            FileName = "test-image.png",
            FileType = "image/png",
            FileSize = 1024,
            UploadedBy = "user-123",
            AccountId = "acc-123",
            ProfileId = "profile-123",
            StorageLocation = "/uploads/test-image.png"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be("test-image.png");
        result.FileType.Should().Be("image/png");
        result.MediaId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistMediaToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UploadMediaCommandHandler(context);
        var command = new UploadMediaCommand
        {
            FileName = "document.pdf",
            FileType = "application/pdf",
            FileSize = 5000,
            UploadedBy = "user-456",
            AccountId = "acc-456",
            ProfileId = "profile-456",
            StorageLocation = "/uploads/document.pdf"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedMedia = await context.Medias.FindAsync(result.MediaId);
        savedMedia.Should().NotBeNull();
        savedMedia!.FileName.Should().Be("document.pdf");
        savedMedia.FileSize.Should().Be(5000);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueMediaId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UploadMediaCommandHandler(context);
        var command1 = new UploadMediaCommand
        {
            FileName = "file1.png",
            FileType = "image/png",
            FileSize = 1000,
            UploadedBy = "user-123",
            AccountId = "acc-123",
            ProfileId = "profile-123",
            StorageLocation = "/uploads/file1.png"
        };
        var command2 = new UploadMediaCommand
        {
            FileName = "file2.png",
            FileType = "image/png",
            FileSize = 2000,
            UploadedBy = "user-123",
            AccountId = "acc-123",
            ProfileId = "profile-123",
            StorageLocation = "/uploads/file2.png"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.MediaId.Should().NotBe(result2.MediaId);
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new UploadMediaCommandHandler(context);
        var command = new UploadMediaCommand
        {
            FileName = "cancel.png",
            FileType = "image/png",
            FileSize = 100,
            UploadedBy = "user-123",
            AccountId = "acc-123",
            ProfileId = "profile-123",
            StorageLocation = "/uploads/cancel.png"
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
