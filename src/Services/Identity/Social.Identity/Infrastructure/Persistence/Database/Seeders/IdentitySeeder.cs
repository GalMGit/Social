
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Social.Contracts.Events.Identity;
using Social.Identity.Domain;
using Social.Identity.Infrastructure.Persistence.Database.Context;
using Social.Shared.Names;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence;

namespace Social.Identity.Infrastructure.Persistence.Database.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IDbContextOutbox<IdentityDbContext> outbox,
        IOptions<AdminOptions> options,
        CancellationToken ct = default)
    {
        var db = outbox.DbContext;
        var adminOptions = options.Value;

        var roleNames = new[]
        {
            RoleNames.User,
            RoleNames.Admin,
            RoleNames.SuperAdmin
        };

        var roles = await db.Roles
            .ToDictionaryAsync(x => x.Name, ct);

        foreach (var roleName in roleNames)
        {
            if (roles.ContainsKey(roleName))
                continue;

            var role = new Role
            {
                Id = Guid.CreateVersion7(),
                Name = roleName
            };

            db.Roles.Add(role);
            roles.Add(roleName, role);
        }

        await SeedSuperAdminAsync(
            outbox,
            roles[RoleNames.SuperAdmin],
            adminOptions,
            ct);

        await outbox.SaveChangesAndFlushMessagesAsync(ct);
    }

    private static async Task SeedSuperAdminAsync(
        IDbContextOutbox<IdentityDbContext> outbox,
        Role superAdminRole,
        AdminOptions options,
        CancellationToken ct)
    {
        var db = outbox.DbContext;

        var adminExists = await db.Users
            .AnyAsync(x => x.Email == options.Email, ct);

        if (adminExists)
            return;

        var admin = new User
        {
            Id = Guid.CreateVersion7(),
            Email = options.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                options.Password),
            CreatedAt = DateTime.UtcNow,
            Roles = [superAdminRole]
        };

        db.Users.Add(admin);

        await outbox.PublishAsync(
            new UserCreatedEvent(
                admin.Id,
                options.Username));
    }
}