using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Social.Shared.Authentication;
using Social.Shared.Endpoint;
using Social.Users.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Persistence.Durability;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Social.Users.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddUsers(
            IConfiguration configuration)
        {
            services.AddDbContextWithWolverineIntegration<
                UsersDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(
                            "UsersDb"));
                });
            
            services.AddEndpoints(
                Assembly.GetExecutingAssembly());
            
            services.AddAuth(configuration);

            return services;
        }
    }

    extension(WolverineOptions options)
    {
        public void AddUsersMessaging(
            IConfiguration configuration)
        {
            options.Discovery.IncludeAssembly(
                Assembly.GetExecutingAssembly());

            options.ListenToRabbitQueue("social-users")
                .ConfigureQueue(q =>
                {
                    q.IsDurable = true;
                    q.AutoDelete = false;
                    q.IsExclusive = false;
                });
        }
    }

    extension(IServiceProvider services)
    {
        public async Task InitializeUsersAsync()
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<UsersDbContext>();

            await db.Database.MigrateAsync();
        }
    }
}
