using System.Reflection;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        
        private void AddAuth(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(
                configuration.GetSection(nameof(JwtOptions)));

            services.AddOptions<JwtOptions>()
                .Validate(o => 
                    !string.IsNullOrEmpty(o.SecretKey), 
                    "SecretKey is required")
                .ValidateOnStart();

            var jwtOptions = configuration
                .GetSection(nameof(JwtOptions))
                .Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                        NameClaimType = ClaimTypes.NameIdentifier,
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var request = context.HttpContext.Request;
                            
                            var authHeader = request.Headers.Authorization.FirstOrDefault();
                            if (!string.IsNullOrEmpty(authHeader) &&
                                authHeader.StartsWith(
                                    "Bearer ", 
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authHeader["Bearer "
                                    .Length..].Trim();
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
        }
    }
    
    extension(WolverineOptions options)
    {
        public void AddIdentityMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());
            
            options.PersistMessagesWithPostgresql(
                    configuration.GetConnectionString(
                        "IdentityDb")!,
                    role: MessageStoreRole.Ancillary)
                .Enroll<IdentityDbContext>();

            options.PublishMessage<UserStartRegistrationEvent>()
                .ToRabbitQueue("social-email");
        }
    }
    
    extension(IServiceProvider services)
    {
        public async Task InitializeIdentityAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();

            var adminOptions = scope.ServiceProvider
                .GetRequiredService<IOptions<AdminOptions>>();

            await db.Database.MigrateAsync();

            await IdentitySeeder.SeedAsync(
                db,
                adminOptions);
        }
    }
}
