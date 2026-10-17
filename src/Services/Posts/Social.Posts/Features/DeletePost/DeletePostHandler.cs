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
        var exists = await context.Posts
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == command.PostId, ct);

        if (!exists)
            return Result.Failure(
                PostErrors.NotFound);

        var canDelete = await context.Posts
            .AsNoTracking()
            .AnyAsync(x => 
                x.AuthorId == command.UserId, ct);

        if (!canDelete)
            return Result.Failure(
                PostErrors.AccessDenied);

        await context.Posts
            .Where(x => 
                x.Id == command.PostId)
            .ExecuteDeleteAsync(ct);

        await bus.PublishAsync(
            new PostDeletedEvent(
                command.PostId));

        return Result.Success();
    }
}