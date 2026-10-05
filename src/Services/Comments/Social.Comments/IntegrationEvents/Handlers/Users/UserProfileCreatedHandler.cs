using Microsoft.EntityFrameworkCore;
using Social.Comments.Domain;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Contracts.Events.Posts;
using Social.Contracts.Events.Users;
using Wolverine.Attributes;

namespace Social.Comments.IntegrationEvents.Handlers.Users;

[Transactional(typeof(CommentsDbContext))]
public sealed class UserProfileCreatedHandler(
    CommentsDbContext context,
    ILogger<UserProfileCreatedHandler> logger)
{
    public async Task Handle(
        UserProfileCreatedEvent @event,
        CancellationToken ct)
    {
        var exists = await context.KnownUsers
            .AnyAsync(x =>
                x.UserId == @event.UserId, ct);

        if (exists) return;

        await context.KnownUsers.AddAsync(new KnownUser
        {
            UserId = @event.UserId,
            Username = @event.Username
        }, ct);

        logger.LogInformation("KnownUser {UserId} added", @event.UserId);
    }
}