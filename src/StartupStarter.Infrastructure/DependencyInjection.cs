using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StartupStarter.Core;

namespace StartupStarter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<StartupStarterContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(StartupStarterContext).Assembly.FullName)));

        // Register IStartupStarterContext
        services.AddScoped<IStartupStarterContext>(provider =>
            provider.GetRequiredService<StartupStarterContext>());

        return services;
    }
}
