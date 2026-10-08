using System.Reflection;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Social.Contracts.Events.Identity;
using Social.Identity.Application.Abstractions.Auth;
using Social.Identity.Application.Abstractions.Cache;
using Social.Identity.Infrastructure.Auth;
using Social.Identity.Infrastructure.Cache;
using Social.Identity.Infrastructure.Persistence.Database.Context;
using Social.Identity.Infrastructure.Persistence.Database.Seeders;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;

namespace Social.Identity.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIdentity(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                IdentityDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "IdentityDb"));
                });
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString(
                    "Redis");
                options.InstanceName = "Social_";
            });
            
            services.AddAuth(configuration);
            
            services.Configure<AdminOptions>(
                configuration.GetSection("Identity:Admin"));
            
            services.AddEndpoints(
                Assembly.GetExecutingAssembly());
            
            services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());
            
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            
            return services;
        }
        
    }
    
    extension(WolverineOptions options)
    {
        public void AddIdentityMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());
            
            options.PublishMessage<UserStartRegistrationEvent>()
                .ToRabbitQueue("social-email");
            
            options.PublishMessage<UserCreatedEvent>()
                .ToRabbitQueue("social-users");
        }
    }
    
    extension(IServiceProvider services)
    {
        public async Task MigrateIdentityAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();

            await db.Database.MigrateAsync();
        }
        
        public async Task SeedIdentityAsync(
            CancellationToken ct = default)
        {
            using var scope = services.CreateScope();

            var outbox = scope.ServiceProvider
                .GetRequiredService<IDbContextOutbox<IdentityDbContext>>();

            var adminOptions = scope.ServiceProvider
                .GetRequiredService<IOptions<AdminOptions>>();

            await IdentitySeeder.SeedAsync(
                outbox,
                adminOptions,
                ct);
        }
    }
}


public static class DatabaseBootstrapper
{
    public static async Task EnsureDatabaseExistsAsync(
        string connectionString,
        CancellationToken ct = default)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var dbName = builder.Database
                     ?? throw new InvalidOperationException("Database name is missing.");

        var adminBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        Console.WriteLine($"[DB-INIT] Ensuring database '{dbName}' exists via {adminBuilder.Host}:{adminBuilder.Port}...");

        await using var conn = new NpgsqlConnection(adminBuilder.ConnectionString);
        await conn.OpenAsync(ct);
        Console.WriteLine("[DB-INIT] Connected to admin database.");

        await using var checkCmd = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @name", conn);
        checkCmd.Parameters.AddWithValue("name", dbName);

        var exists = await checkCmd.ExecuteScalarAsync(ct) is not null;
        if (exists)
        {
            Console.WriteLine($"[DB-INIT] Database '{dbName}' already exists.");
            return;
        }

        Console.WriteLine($"[DB-INIT] Creating database '{dbName}'...");
        var safeName = dbName.Replace("\"", "\"\"");
        await using var createCmd = new NpgsqlCommand(
            $"CREATE DATABASE \"{safeName}\"", conn);
        await createCmd.ExecuteNonQueryAsync(ct);
        Console.WriteLine($"[DB-INIT] Database '{dbName}' created.");
    }
}
