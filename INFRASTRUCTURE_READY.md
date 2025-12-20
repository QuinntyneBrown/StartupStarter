# Infrastructure Layer - Ready to Implement

## IStartupStarterContext - ✅ Complete

The `IStartupStarterContext` interface is now fully configured with all implemented aggregates:

### Currently Active DbSets

```csharp
// Account Management
DbSet<Account> Accounts
DbSet<AccountSettings> AccountSettings

// User Management
DbSet<User> Users
DbSet<UserInvitation> UserInvitations

// Profile Management
DbSet<Profile> Profiles
DbSet<ProfilePreferences> ProfilePreferences
DbSet<ProfileShare> ProfileShares
```

**Total: 7 DbSets ready for use** ✅

### Placeholders for Future Aggregates

The interface includes commented placeholders for the remaining 12 aggregates:
- Role Management (2 entities)
- Content Management (2 entities)
- Dashboard Management (3 entities)
- Media Management (1 entity)
- API Management (4 entities)
- Audit (3 entities)
- Authentication (4 entities)
- Workflow (3 entities)
- System aggregates (3 entities)

## Next Step: Implement StartupStarterContext

Create `src/StartupStarter.Infrastructure/StartupStarterContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;
using StartupStarter.Core.Model.AccountAggregate.Entities;
using StartupStarter.Core.Model.UserAggregate.Entities;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure;

public class StartupStarterContext : DbContext, IStartupStarterContext
{
    public StartupStarterContext(DbContextOptions<StartupStarterContext> options)
        : base(options)
    {
    }

    // Account Management
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<AccountSettings> AccountSettings { get; set; } = null!;

    // User Management
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserInvitation> UserInvitations { get; set; } = null!;

    // Profile Management
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<ProfilePreferences> ProfilePreferences { get; set; } = null!;
    public DbSet<ProfileShare> ProfileShares { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StartupStarterContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Publish domain events before saving
        var events = ChangeTracker.Entries<Account>()
            .SelectMany(e => e.Entity.DomainEvents)
            .Concat(ChangeTracker.Entries<User>()
                .SelectMany(e => e.Entity.DomainEvents))
            .Concat(ChangeTracker.Entries<Profile>()
                .SelectMany(e => e.Entity.DomainEvents))
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Clear domain events after saving
        foreach (var entry in ChangeTracker.Entries<Account>())
            entry.Entity.ClearDomainEvents();
        foreach (var entry in ChangeTracker.Entries<User>())
            entry.Entity.ClearDomainEvents();
        foreach (var entry in ChangeTracker.Entries<Profile>())
            entry.Entity.ClearDomainEvents();

        // TODO: Publish events to event bus/mediator

        return result;
    }
}
```

## Entity Configurations to Create

Create these files in `src/StartupStarter.Infrastructure/EntityConfigurations/`:

### 1. AccountConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AccountAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.AccountId);

        builder.Property(a => a.AccountId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.AccountName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.OwnerUserId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.SubscriptionTier).IsRequired().HasMaxLength(50);
        builder.Property(a => a.SuspensionReason).HasMaxLength(500);

        builder.HasMany(a => a.Settings)
            .WithOne(s => s.Account)
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.DomainEvents);

        builder.HasIndex(a => a.AccountName).IsUnique();
        builder.HasIndex(a => a.OwnerUserId);
        builder.HasIndex(a => a.Status);
    }
}
```

### 2. AccountSettingsConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AccountAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class AccountSettingsConfiguration : IEntityTypeConfiguration<AccountSettings>
{
    public void Configure(EntityTypeBuilder<AccountSettings> builder)
    {
        builder.ToTable("AccountSettings");
        builder.HasKey(s => s.AccountSettingsId);

        builder.Property(s => s.AccountSettingsId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.AccountId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
        builder.Property(s => s.SettingsJson).IsRequired();

        builder.HasIndex(s => new { s.AccountId, s.Category });
    }
}
```

### 3. UserConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.AccountId).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(u => u.LockReason).HasMaxLength(500);

        builder.Ignore(u => u.DomainEvents);
        builder.Ignore(u => u.RoleIds);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.AccountId);
        builder.HasIndex(u => u.Status);
    }
}
```

### 4. UserInvitationConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class UserInvitationConfiguration : IEntityTypeConfiguration<UserInvitation>
{
    public void Configure(EntityTypeBuilder<UserInvitation> builder)
    {
        builder.ToTable("UserInvitations");
        builder.HasKey(i => i.InvitationId);

        builder.Property(i => i.InvitationId).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(256);
        builder.Property(i => i.AccountId).IsRequired().HasMaxLength(100);
        builder.Property(i => i.InvitedBy).IsRequired().HasMaxLength(100);
        builder.Property(i => i.AcceptedByUserId).HasMaxLength(100);

        builder.Ignore(i => i.DomainEvents);
        builder.Ignore(i => i.RoleIds);
        builder.Ignore(i => i.IsExpired);

        builder.HasIndex(i => i.Email);
        builder.HasIndex(i => i.AccountId);
        builder.HasIndex(i => new { i.IsAccepted, i.ExpiresAt });
    }
}
```

### 5. ProfileConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");
        builder.HasKey(p => p.ProfileId);

        builder.Property(p => p.ProfileId).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ProfileName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.AccountId).IsRequired().HasMaxLength(100);
        builder.Property(p => p.CreatedBy).IsRequired().HasMaxLength(100);
        builder.Property(p => p.AvatarUrl).HasMaxLength(500);

        builder.HasMany(p => p.Preferences)
            .WithOne(pref => pref.Profile)
            .HasForeignKey(pref => pref.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Shares)
            .WithOne(s => s.Profile)
            .HasForeignKey(s => s.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.DomainEvents);
        builder.Ignore(p => p.DashboardIds);

        builder.HasIndex(p => new { p.AccountId, p.ProfileName }).IsUnique();
        builder.HasIndex(p => p.CreatedBy);
    }
}
```

### 6. ProfilePreferencesConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ProfilePreferencesConfiguration : IEntityTypeConfiguration<ProfilePreferences>
{
    public void Configure(EntityTypeBuilder<ProfilePreferences> builder)
    {
        builder.ToTable("ProfilePreferences");
        builder.HasKey(p => p.ProfilePreferencesId);

        builder.Property(p => p.ProfilePreferencesId).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ProfileId).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PreferencesJson).IsRequired();

        builder.HasIndex(p => new { p.ProfileId, p.Category });
    }
}
```

### 7. ProfileShareConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ProfileShareConfiguration : IEntityTypeConfiguration<ProfileShare>
{
    public void Configure(EntityTypeBuilder<ProfileShare> builder)
    {
        builder.ToTable("ProfileShares");
        builder.HasKey(s => s.ProfileShareId);

        builder.Property(s => s.ProfileShareId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.ProfileId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.OwnerUserId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.SharedWithUserId).IsRequired().HasMaxLength(100);

        builder.HasIndex(s => new { s.ProfileId, s.SharedWithUserId }).IsUnique();
        builder.HasIndex(s => s.OwnerUserId);
    }
}
```

## Create Migration Commands

Once all entity configurations are created:

```bash
# Navigate to API project (startup project)
cd src/StartupStarter.Api

# Add connection string to appsettings.json first
# Then create initial migration
dotnet ef migrations add InitialCreate --project ../StartupStarter.Infrastructure

# Apply migration to database
dotnet ef database update
```

## Connection String

Add to `src/StartupStarter.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StartupStarter;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Dependency Injection Setup

Update `src/StartupStarter.Api/Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using StartupStarter.Core;
using StartupStarter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<StartupStarterContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IStartupStarterContext
builder.Services.AddScoped<IStartupStarterContext>(provider =>
    provider.GetRequiredService<StartupStarterContext>());

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Verification Checklist

- [ ] StartupStarterContext.cs created
- [ ] 7 Entity Configuration files created
- [ ] Connection string added to appsettings.json
- [ ] Program.cs configured with DI
- [ ] Migration created successfully
- [ ] Database updated successfully
- [ ] Infrastructure project builds without errors

## Status

**IStartupStarterContext:** ✅ Complete - Ready for Infrastructure implementation
**Entities Ready:** 7 (3 aggregates fully implemented)
**Next:** Create Infrastructure layer following this guide
