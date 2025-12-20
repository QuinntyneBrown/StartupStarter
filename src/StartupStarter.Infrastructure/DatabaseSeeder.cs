using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StartupStarter.Core;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.AccountAggregate.Enums;
using StartupStarter.Core.Model.RoleAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Enums;

namespace StartupStarter.Infrastructure;

public class DatabaseSeeder
{
    private readonly IStartupStarterContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(IStartupStarterContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            if (_context is DbContext dbContext)
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            // Check if admin user already exists
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "admin");

            if (adminUser != null)
            {
                _logger.LogInformation("Admin user already exists. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Create admin account
            var accountId = Guid.NewGuid().ToString();
            var account = new Account(
                accountId,
                "Admin Account",
                AccountType.Enterprise,
                "system",
                "Enterprise",
                "system"
            );
            _context.Accounts.Add(account);

            // Create admin role
            var roleId = Guid.NewGuid().ToString();
            var adminRole = new Role(
                roleId,
                "Administrator",
                "Full system administrator with all permissions",
                accountId,
                new List<string> { "*" }, // All permissions
                "system"
            );
            _context.Roles.Add(adminRole);

            // Create admin user
            var userId = Guid.NewGuid().ToString();
            var user = new User(
                userId,
                "admin@gmail.com",
                "Admin",
                "User",
                accountId,
                "admin", // Simple password hash (in production, use BCrypt)
                new List<string> { roleId },
                "system",
                false
            );

            // Activate the admin user
            user.Activate("system", ActivationMethod.AdminActivation);

            _context.Users.Add(user);

            //// Create UserRole mapping
            //var userRole = new UserRole(
            //    Guid.NewGuid().ToString(),
            //    userId,
            //    roleId,
            //    accountId,
            //    "system"
            //);
            //_context.UserRoles.Add(userRole);

            // Save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully.");
            _logger.LogInformation("Admin user created - Email: admin, Password: admin");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
