using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Communities;
using Social.Contracts.Events.Posts;
using Social.Posts.Application.Errors;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Posts.Features.DeletePost;

public sealed class DeletePostHandler(
    PostsDbContext context,
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

        return Result.Success();
    }
}