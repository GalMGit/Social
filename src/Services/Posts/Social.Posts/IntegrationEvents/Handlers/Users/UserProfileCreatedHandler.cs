using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Users;
using Social.Posts.Domain;
using Social.Posts.Infrastructure.Persistence.Context;
using Wolverine.Attributes;

namespace Social.Posts.IntegrationEvents.Handlers.Users;

[Transactional(typeof(PostsDbContext))]
public sealed class UserProfileCreatedHandler(
    PostsDbContext context,
    ILogger<UserProfileCreatedHandler> logger)
{
    public async Task Handle(
        UserProfileCreatedEvent @event,
        CancellationToken ct)
    {
        var exists = await context.KnownUsers
            .AnyAsync(x =>
                x.UserId == @event.UserId, ct);

        if (exists) 
            return;

        await context.KnownUsers.AddAsync(new KnownUser
        {
            UserId = @event.UserId,
            Username = @event.Username
        }, ct);

        logger.LogInformation("KnownUser {UserId} added", @event.UserId);
    }
}