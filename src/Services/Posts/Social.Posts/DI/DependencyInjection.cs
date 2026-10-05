using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Posts;
using Social.Posts.Application.Media;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Social.Posts.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPosts(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                PostsDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "PostsDb"));
                });
            
            services.AddAuth(configuration);

            services.AddEndpoints(
                Assembly.GetExecutingAssembly());
            
            services.Configure<MediaOptions>(
                configuration.GetSection(
                    nameof(MediaOptions)));
            
            services.AddSingleton<IMediaUrlBuilder, MediaUrlBuilder>();

            services.AddValidatorsFromAssembly(
                Assembly.GetExecutingAssembly());

            return services;
        }
    }

    extension(WolverineOptions options)
    {
        public void AddPostsMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());

            options.PublishMessage<PostCreatedEvent>()
                .ToRabbitExchange("posts-events");
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializePostsAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<PostsDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
