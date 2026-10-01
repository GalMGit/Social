using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Identity;
using Social.Users.Domain;
using Social.Users.Infrastructure.Persistence.Context;
using Wolverine.Attributes;

namespace Social.Users.IntegrationsEvents.Handlers;

[Transactional(typeof(UsersDbContext))]
public sealed class UserCreatedHandler(
    UsersDbContext context,
    ILogger<UserCreatedHandler> logger)
{
    public async Task Handle(
        UserCreatedEvent @event,
        CancellationToken ct)
    {
        var exists = await context.Profiles
            .AnyAsync(x => x.Id == @event.UserId, ct);

        if (exists)
        {
            logger.LogInformation(
                "Profile for user {UserId} already exists, skipping",
                @event.UserId);
            return;
        }

        var profile = new UserProfile
        {
            Id = @event.UserId,
            Username = @event.Username,
            CreatedAt = DateTime.UtcNow
        };

        await context.Profiles.AddAsync(
            profile, ct);

        logger.LogInformation(
            "Profile created for user {UserId}",
            @event.UserId);
    }
}
