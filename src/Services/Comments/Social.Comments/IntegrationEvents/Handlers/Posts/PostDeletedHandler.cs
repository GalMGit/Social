using Microsoft.EntityFrameworkCore;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Contracts.Events.Posts;
using Wolverine.Attributes;

namespace Social.Comments.IntegrationEvents.Handlers.Posts;

[Transactional(typeof(CommentsDbContext))]
public sealed class PostDeletedHandler(
    CommentsDbContext context,
    ILogger<PostDeletedHandler> logger)
{
    public async Task Handle(
        PostDeletedEvent @event, 
        CancellationToken ct)
    {
        await context.KnownPosts
            .Where(x => x.PostId == @event.PostId)
            .ExecuteDeleteAsync(ct);

        var removed = await context.Comments
            .Where(x => x.PostId == @event.PostId)
            .ExecuteDeleteAsync(ct);

        logger.LogInformation(
            "Post {PostId} removed, {Count} comments deleted",
            @event.PostId, removed);
    }
}