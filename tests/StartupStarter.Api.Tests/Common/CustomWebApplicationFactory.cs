using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StartupStarter.Core;
using StartupStarter.Infrastructure;

namespace StartupStarter.Api.Tests.Common;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StartupStarterContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove the existing IStartupStarterContext registration
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IStartupStarterContext));

            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<StartupStarterContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
            });

            services.AddScoped<IStartupStarterContext>(provider =>
                provider.GetRequiredService<StartupStarterContext>());

            // Ensure the database is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<StartupStarterContext>();
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}
