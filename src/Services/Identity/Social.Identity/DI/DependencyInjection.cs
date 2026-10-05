using System.Reflection;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
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
        public async Task InitializeIdentityAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();
            
            var outbox = scope.ServiceProvider
                .GetRequiredService<IDbContextOutbox<IdentityDbContext>>();

            var adminOptions = scope.ServiceProvider
                .GetRequiredService<IOptions<AdminOptions>>();

            await db.Database.MigrateAsync();

            await IdentitySeeder.SeedAsync(
                outbox,
                adminOptions);
        }
    }
}
