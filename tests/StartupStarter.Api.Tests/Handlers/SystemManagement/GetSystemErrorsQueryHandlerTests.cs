using FluentAssertions;
using StartupStarter.Api.Features.SystemManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.SystemAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.SystemManagement;

public class GetSystemErrorsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingErrors_ShouldReturnErrors()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var error1 = new SystemError(
            "error-1",
            "NullReferenceException",
            "Object reference not set",
            "API.Controllers.UserController",
            "at line 45"
        );
        var error2 = new SystemError(
            "error-2",
            "InvalidOperationException",
            "Invalid state",
            "API.Services.AuthService",
            "at line 120"
        );
        context.SystemErrors.Add(error1);
        context.SystemErrors.Add(error2);
        await context.SaveChangesAsync();

        var handler = new GetSystemErrorsQueryHandler(context);
        var query = new GetSystemErrorsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoErrors_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetSystemErrorsQueryHandler(context);
        var query = new GetSystemErrorsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorsInDescendingOrder()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var error1 = new SystemError("error-1", "OldError", "Old", "Source1", "trace1");
        context.SystemErrors.Add(error1);
        await context.SaveChangesAsync();
        await Task.Delay(10);
        var error2 = new SystemError("error-2", "NewError", "New", "Source2", "trace2");
        context.SystemErrors.Add(error2);
        await context.SaveChangesAsync();

        var handler = new GetSystemErrorsQueryHandler(context);
        var query = new GetSystemErrorsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].ErrorType.Should().Be("NewError");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetSystemErrorsQueryHandler(context);
        var query = new GetSystemErrorsQuery();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
