using FluentAssertions;
using StartupStarter.Api.Features.SystemManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.BackupAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.SystemManagement;

public class StartBackupCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldStartBackup()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartBackupCommandHandler(context);
        var command = new StartBackupCommand
        {
            BackupType = BackupType.Full
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.BackupId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistBackupToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartBackupCommandHandler(context);
        var command = new StartBackupCommand
        {
            BackupType = BackupType.Incremental
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedBackup = await context.SystemBackups.FindAsync(result.BackupId);
        savedBackup.Should().NotBeNull();
        savedBackup!.BackupType.Should().Be(BackupType.Incremental);
    }

    [Theory]
    [InlineData(BackupType.Full)]
    [InlineData(BackupType.Incremental)]
    [InlineData(BackupType.Differential)]
    public async Task Handle_WithDifferentBackupTypes_ShouldCreateCorrectType(BackupType backupType)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartBackupCommandHandler(context);
        var command = new StartBackupCommand
        {
            BackupType = backupType
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.BackupType.Should().Be(backupType.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new StartBackupCommandHandler(context);
        var command = new StartBackupCommand
        {
            BackupType = BackupType.Full
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
