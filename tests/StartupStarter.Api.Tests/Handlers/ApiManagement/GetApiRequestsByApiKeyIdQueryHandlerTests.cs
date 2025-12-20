using FluentAssertions;
using StartupStarter.Api.Features.ApiManagement.Queries;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Api.Tests.Handlers.ApiManagement;

public class GetApiRequestsByApiKeyIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingApiRequests_ShouldReturnRequests()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var request1 = new ApiRequest(
            "req-1",
            "key-123",
            "/api/users",
            "GET",
            200,
            100
        );
        var request2 = new ApiRequest(
            "req-2",
            "key-123",
            "/api/accounts",
            "POST",
            201,
            150
        );
        context.ApiRequests.Add(request1);
        context.ApiRequests.Add(request2);
        await context.SaveChangesAsync();

        var handler = new GetApiRequestsByApiKeyIdQueryHandler(context);
        var query = new GetApiRequestsByApiKeyIdQuery { ApiKeyId = "key-123" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoApiRequests_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetApiRequestsByApiKeyIdQueryHandler(context);
        var query = new GetApiRequestsByApiKeyIdQuery { ApiKeyId = "key-with-no-requests" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnRequestsInDescendingOrder()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var request1 = new ApiRequest(
            "req-1",
            "key-456",
            "/api/old",
            "GET",
            200,
            100
        );
        var request2 = new ApiRequest(
            "req-2",
            "key-456",
            "/api/new",
            "GET",
            200,
            50
        );
        context.ApiRequests.Add(request1);
        await context.SaveChangesAsync();
        await Task.Delay(10); // Small delay to ensure different timestamps
        context.ApiRequests.Add(request2);
        await context.SaveChangesAsync();

        var handler = new GetApiRequestsByApiKeyIdQueryHandler(context);
        var query = new GetApiRequestsByApiKeyIdQuery { ApiKeyId = "key-456" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Endpoint.Should().Be("/api/new");
    }

    [Fact]
    public async Task Handle_ShouldSupportCancellation()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var handler = new GetApiRequestsByApiKeyIdQueryHandler(context);
        var query = new GetApiRequestsByApiKeyIdQuery { ApiKeyId = "key-123" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = () => handler.Handle(query, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
