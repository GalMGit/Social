using Microsoft.EntityFrameworkCore;
using Social.Comments.Domain;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Contracts.Events.Posts;
using Wolverine.Attributes;

namespace Social.Comments.IntegrationEvents.Handlers.Posts;

[Transactional(typeof(CommentsDbContext))]
public sealed class PostCreatedHandler(
    CommentsDbContext context,
    ILogger<PostCreatedHandler> logger)
{
    public async Task Handle(
        PostCreatedEvent @event,
        CancellationToken ct)
    {
        var exists = await context.KnownPosts
            .AnyAsync(x =>
                x.PostId == @event.PostId, ct);

        if (exists) return;

        await context.KnownPosts.AddAsync(new KnownPost
        {
            PostId = @event.PostId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        logger.LogInformation("KnownPost {PostId} added", @event.PostId);
    }
}