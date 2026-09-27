using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Social.Identity.Domain;
using Social.Identity.Infrastructure.Persistence.Database.Context;
using Social.Shared.Names;

namespace Social.Identity.Infrastructure.Persistence.Database.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IdentityDbContext db,
        IOptions<AdminOptions> options,
        CancellationToken ct = default)
    {
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

        await db.SaveChangesAsync(ct);

        await SeedSuperAdminAsync(
            db,
            roles[RoleNames.SuperAdmin],
            adminOptions,
            ct);
    }

    private static async Task SeedSuperAdminAsync(
        IdentityDbContext db,
        Role superAdminRole,
        AdminOptions options,
        CancellationToken ct)
    {
        var adminExists = await db.Users
            .AnyAsync(x => x.Email == options.Email, ct);

        if (adminExists)
            return;

        var admin = new User
        {
            Id = Guid.CreateVersion7(),
            Username = options.Username,
            Email = options.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(options.Password),
            CreatedAt = DateTime.UtcNow,
            Roles = [superAdminRole]
        };

        db.Users.Add(admin);

        await db.SaveChangesAsync(ct);
    }
}