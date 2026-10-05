using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;
using Wolverine;
using Wolverine.EntityFrameworkCore;


namespace Social.Comments.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddComments(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                CommentsDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "CommentsDb"));
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
        public void AddCommentsMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());
            
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializeCommentsAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<CommentsDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
