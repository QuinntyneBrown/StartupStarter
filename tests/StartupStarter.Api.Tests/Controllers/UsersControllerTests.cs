using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StartupStarter.Api.Features.UserManagement.Commands;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Api.Tests.Common;
using StartupStarter.Core;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Api.Tests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "controller-test@example.com",
            FirstName = "Controller",
            LastName = "Test",
            AccountId = "acc-123",
            PasswordHash = "hashedPassword",
            InitialRoles = new List<string> { "user" },
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        result.Should().NotBeNull();
        result!.Email.Should().Be("controller-test@example.com");
        result.FirstName.Should().Be("Controller");
        result.LastName.Should().Be("Test");
        result.Status.Should().Be(UserStatus.Invited.ToString());
    }

    [Fact]
    public async Task CreateUser_ShouldReturnLocationHeader()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "location-test@example.com",
            FirstName = "Location",
            LastName = "Test",
            AccountId = "acc-123",
            PasswordHash = "hash",
            InitialRoles = new List<string>(),
            CreatedBy = "admin",
            InvitationSent = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/users/");
    }

    #endregion

    #region GetUser Tests

    [Fact]
    public async Task GetUser_WithExistingUser_ShouldReturnOk()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IStartupStarterContext>();

        var userId = Guid.NewGuid().ToString();
        var user = new User(
            userId,
            "getuser@example.com",
            "Get",
            "User",
            "acc-123",
            "hash",
            new List<string>(),
            "admin",
            true);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Email.Should().Be("getuser@example.com");
    }

    [Fact]
    public async Task GetUser_WithNonExistingUser_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/non-existing-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region ActivateUser Tests

    [Fact]
    public async Task ActivateUser_WithValidData_ShouldReturnOk()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IStartupStarterContext>();

        var userId = Guid.NewGuid().ToString();
        var user = new User(
            userId,
            "activate@example.com",
            "Activate",
            "User",
            "acc-123",
            "hash",
            new List<string>(),
            "admin",
            true);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var command = new ActivateUserCommand
        {
            UserId = userId,
            ActivatedBy = "admin",
            Method = ActivationMethod.AdminActivation
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/users/{userId}/activate", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        result.Should().NotBeNull();
        result!.Status.Should().Be(UserStatus.Active.ToString());
    }

    #endregion
}
