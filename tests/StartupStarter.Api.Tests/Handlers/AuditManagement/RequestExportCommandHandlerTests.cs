using FluentAssertions;
using StartupStarter.Api.Features.AuditManagement.Commands;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuditAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuditManagement;

public class RequestExportCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateExportRequest()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RequestExportCommandHandler(context);
        var command = new RequestExportCommand
        {
            AccountId = "acc-123",
            RequestedBy = "admin",
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow,
            Filters = new Dictionary<string, object> { { "entityType", "User" } },
            FileFormat = FileFormat.CSV
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be("acc-123");
        result.RequestedBy.Should().Be("admin");
        result.ExportId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistExportToDatabase()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RequestExportCommandHandler(context);
        var command = new RequestExportCommand
        {
            AccountId = "acc-456",
            RequestedBy = "analyst",
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            Filters = new Dictionary<string, object>(),
            FileFormat = FileFormat.JSON
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedExport = await context.AuditExports.FindAsync(result.ExportId);
        savedExport.Should().NotBeNull();
        savedExport!.AccountId.Should().Be("acc-456");
        savedExport.RequestedBy.Should().Be("analyst");
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateUniqueExportId()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RequestExportCommandHandler(context);
        var command1 = new RequestExportCommand
        {
            AccountId = "acc-123",
            RequestedBy = "admin",
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow,
            Filters = new Dictionary<string, object>(),
            FileFormat = FileFormat.CSV
        };
        var command2 = new RequestExportCommand
        {
            AccountId = "acc-123",
            RequestedBy = "admin",
            StartDate = DateTime.UtcNow.AddDays(-60),
            EndDate = DateTime.UtcNow.AddDays(-30),
            Filters = new Dictionary<string, object>(),
            FileFormat = FileFormat.CSV
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.ExportId.Should().NotBe(result2.ExportId);
    }

    [Theory]
    [InlineData(FileFormat.CSV)]
    [InlineData(FileFormat.JSON)]
    [InlineData(FileFormat.PDF)]
    public async Task Handle_WithDifferentFileFormats_ShouldCreateCorrectFormat(FileFormat format)
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RequestExportCommandHandler(context);
        var command = new RequestExportCommand
        {
            AccountId = "acc-123",
            RequestedBy = "admin",
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow,
            Filters = new Dictionary<string, object>(),
            FileFormat = format
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileFormat.Should().Be(format.ToString());
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new RequestExportCommandHandler(context);
        var command = new RequestExportCommand
        {
            AccountId = "acc-123",
            RequestedBy = "admin",
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow,
            Filters = new Dictionary<string, object>(),
            FileFormat = FileFormat.CSV
        };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(command, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
