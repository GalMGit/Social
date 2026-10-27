using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Social.Contracts.Events.Communities;
using Social.Contracts.Events.Posts;
using Social.Posts.Application.Caching;
using Social.Posts.Application.Errors;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;
using Wolverine;
using Wolverine.Attributes;

namespace Social.Posts.Features.DeletePost;

[Transactional(typeof(PostsDbContext))]
public sealed class DeletePostHandler(
    PostsDbContext context,
    IDistributedCache cache,
    IMessageBus bus)
{
    public async Task<Result> Handle(
        DeletePostCommand command, 
        CancellationToken ct)
    {
        var post = await context.Posts
            .AsNoTracking()
            .Where(x => x.Id == command.PostId)
            .Select(x => new
            {
                x.Id,
                x.AuthorId,
                x.ImagePath,
                x.ThumbnailImagePath
            })
            .FirstOrDefaultAsync(ct);

        if (post is null)
            return Result.Failure(
                PostErrors.NotFound);

        if (post.AuthorId != command.UserId)
            return Result.Failure(
                PostErrors.DeleteAccessDenied);

        await context.Posts
            .Where(x => 
                x.Id == command.PostId)
            .ExecuteDeleteAsync(ct);

        var mediaKeys = new[]
            {
                post.ImagePath,
                post.ThumbnailImagePath
            }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .ToArray();

        await bus.PublishAsync(
            new PostDeletedEvent(
                post.Id,
                mediaKeys));
        
        await cache.RemoveAsync(PostCacheKeys.ById(post.Id), ct);
        await cache.RemoveAsync(PostCacheKeys.All, ct);

        return Result.Success();
    }
}