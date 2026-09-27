using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Social.Identity.Application.Abstractions.Auth;
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
        AdminOptions adminOptions = options.Value;
        var permissionNames = new[]
        {
            PermissionNames.PostsRead,
            PermissionNames.PostsWrite,
            PermissionNames.PostsManage,
            
            PermissionNames.CommunitiesRead,
            PermissionNames.CommunitiesWrite,
            PermissionNames.CommunitiesManage,
            
            PermissionNames.CommentsRead,
            PermissionNames.CommentsWrite,
            PermissionNames.CommentsManage,
            
            PermissionNames.UsersRead,
            PermissionNames.UsersManage
        };

        var permissions = await db.Permissions
            .ToDictionaryAsync(x => x.Name, ct);

        foreach (var permissionName in permissionNames)
        {
            if (permissions.ContainsKey(permissionName))
                continue;

            var permission = new Permission
            {
                Id = Guid.CreateVersion7(),
                Name = permissionName
            };

            db.Permissions.Add(permission);
            permissions.Add(permissionName, permission);
        }

        var roleNames = new[]
        {
            RoleNames.User,
            RoleNames.Admin,
            RoleNames.SuperAdmin,
        };

        var roles = await db.Roles
            .Include(x => x.Permissions)
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
        
        
        await AddPermissionsAsync(
            roles[RoleNames.Admin],
            permissions,
            [
                PermissionNames.PostsRead,
                PermissionNames.PostsWrite,
                PermissionNames.PostsManage,
                PermissionNames.CommentsRead,
                PermissionNames.CommentsWrite,
                PermissionNames.CommentsManage,
                PermissionNames.CommunitiesRead,
                PermissionNames.CommunitiesWrite,
                PermissionNames.CommunitiesManage,
                PermissionNames.UsersManage,
                PermissionNames.UsersRead
            ]);

        await AddPermissionsAsync(
            roles[RoleNames.User],
            permissions,
            [
                PermissionNames.PostsRead,
                PermissionNames.PostsWrite,
                PermissionNames.CommentsRead,
                PermissionNames.CommentsWrite,
                PermissionNames.CommunitiesRead,
                PermissionNames.CommunitiesWrite
            ]);
        
        await AddPermissionsAsync(
            roles[RoleNames.SuperAdmin],
            permissions,
            permissions.Keys);

        await db.SaveChangesAsync(ct);
        
        await SeedSuperAdminAsync(db,
            roles[RoleNames.SuperAdmin],
            adminOptions,
            ct);
    }

    private static Task AddPermissionsAsync(
        Role role,
        IReadOnlyDictionary<string, Permission> permissions,
        IEnumerable<string> permissionNames)
    {
        var existingPermissionIds = role.Permissions
            .Select(x => x.Id)
            .ToHashSet();

        foreach (var permissionName in permissionNames)
        {
            var permission = permissions[permissionName];

            if (existingPermissionIds.Contains(permission.Id))
                continue;

            role.Permissions.Add(permission);
        }

        return Task.CompletedTask;
    }
    
    private static async Task SeedSuperAdminAsync(
        IdentityDbContext db,
        Role adminRole,
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
            Roles = [adminRole]
        };

        db.Users.Add(admin);

        await db.SaveChangesAsync(ct);
    }
}