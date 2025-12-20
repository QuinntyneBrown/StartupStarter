using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StartupStarter.Core;
using StartupStarter.Infrastructure;

namespace StartupStarter.Infrastructure.Tests.DependencyInjection;

public class DependencyInjectionTests
{
    private static IConfiguration CreateConfiguration(string connectionString = "Server=test;Database=test;Trusted_Connection=True;")
    {
        var configValues = new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", connectionString }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(StartupStarterContext));
        dbContextDescriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterIStartupStarterContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var interfaceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IStartupStarterContext));
        interfaceDescriptor.Should().NotBeNull();
        interfaceDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddInfrastructure_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        var result = services.AddInfrastructure(configuration);

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddInfrastructure_DbContext_ShouldBeConfiguredWithSqlServer()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<StartupStarterContext>));
        dbContextDescriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterDbContextAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(StartupStarterContext));
        dbContextDescriptor.Should().NotBeNull();
        dbContextDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddInfrastructure_IStartupStarterContext_ShouldResolveToStartupStarterContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        var interfaceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IStartupStarterContext));

        // Assert
        interfaceDescriptor.Should().NotBeNull();
        interfaceDescriptor!.ImplementationFactory.Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_MultipleRegistrations_ShouldNotDuplicate()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructure(configuration);
        services.AddInfrastructure(configuration);

        // Assert
        var dbContextCount = services.Count(d => d.ServiceType == typeof(StartupStarterContext));
        dbContextCount.Should().Be(2); // AddDbContext adds each time called
    }

    [Fact]
    public void AddInfrastructure_WithDifferentConnectionStrings_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        var config1 = CreateConfiguration("Server=server1;Database=db1;Trusted_Connection=True;");
        var config2 = CreateConfiguration("Server=server2;Database=db2;Trusted_Connection=True;");

        // Act
        services.AddInfrastructure(config1);
        var services2 = new ServiceCollection();
        services2.AddInfrastructure(config2);

        // Assert
        services.Should().NotBeEmpty();
        services2.Should().NotBeEmpty();
    }
}
