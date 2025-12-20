using FluentAssertions;
using StartupStarter.Api.Features.AuditManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.AuditAggregate.Entities;
using StartupStarter.Core.Model.AuditAggregate.Enums;

namespace StartupStarter.Api.Tests.Handlers.AuditManagement;

public class GetAuditExportByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingExport_ShouldReturnExport()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var auditExport = new AuditExport(
            "export-123",
            "acc-123",
            "admin",
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow,
            new Dictionary<string, object>(),
            FileFormat.CSV
        );
        context.AuditExports.Add(auditExport);
        await context.SaveChangesAsync();

        var handler = new GetAuditExportByIdQueryHandler(context);
        var query = new GetAuditExportByIdQuery { ExportId = "export-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ExportId.Should().Be("export-123");
        result.AccountId.Should().Be("acc-123");
    }

    [Fact]
    public async Task Handle_WithNonExistingExport_ShouldReturnNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetAuditExportByIdQueryHandler(context);
        var query = new GetAuditExportByIdQuery { ExportId = "non-existent-export" };

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
        var handler = new GetAuditExportByIdQueryHandler(context);
        var query = new GetAuditExportByIdQuery { ExportId = "export-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
