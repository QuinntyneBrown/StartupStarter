using Microsoft.EntityFrameworkCore;
using StartupStarter.Infrastructure;

namespace StartupStarter.Api.Tests.Common;

public static class TestDbContextFactory
{
    public static StartupStarterContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<StartupStarterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new StartupStarterContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static StartupStarterContext CreateInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<StartupStarterContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new StartupStarterContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
