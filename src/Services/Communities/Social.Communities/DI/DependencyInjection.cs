using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Social.Communities.Infrastructure.Persistence.Context;
using Social.Contracts.Events.Communities;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;

namespace Social.Communities.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCommunities(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                CommunitiesDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "CommunitiesDb"));
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    configuration.GetConnectionString("Redis");
                options.InstanceName = "Social.Communities:";
            });

            services.AddAuth(configuration);

            services.AddEndpoints(
                Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());

            return services;
        }
    }

    extension(WolverineOptions options)
    {
        public void AddCommunitiesMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());
            
            options.PublishMessage<CommunityCreatedEvent>()
                .ToRabbitExchange("communities-events");
            
            options.PublishMessage<CommunityDeletedEvent>()
                .ToRabbitExchange("communities-events");
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializeCommunitiesAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<CommunitiesDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
