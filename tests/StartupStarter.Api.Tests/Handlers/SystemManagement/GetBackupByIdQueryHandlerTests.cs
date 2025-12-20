using FluentAssertions;
using StartupStarter.Api.Features.SystemManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.BackupAggregate.Entities;
using StartupStarter.Core.Model.BackupAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.SystemManagement;

public class GetBackupByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingBackup_ShouldReturnBackup()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var backup = new SystemBackup("backup-123", BackupType.Full);
        context.SystemBackups.Add(backup);
        await context.SaveChangesAsync();

        var handler = new GetBackupByIdQueryHandler(context);
        var query = new GetBackupByIdQuery { BackupId = "backup-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.BackupId.Should().Be("backup-123");
        result.BackupType.Should().Be(BackupType.Full.ToString());
    }

    [Fact]
    public async Task Handle_WithNonExistingBackup_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetBackupByIdQueryHandler(context);
        var query = new GetBackupByIdQuery { BackupId = "non-existent-backup" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetBackupByIdQueryHandler(context);
        var query = new GetBackupByIdQuery { BackupId = "backup-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
