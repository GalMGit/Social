using Microsoft.EntityFrameworkCore;
using Social.Comments.Application.Errors;
using Social.Comments.Domain;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;
using Wolverine.Attributes;

namespace Social.Comments.Features.CreateComment;

[Transactional(typeof(CommentsDbContext))]
public sealed class CreateCommentHandler(
    CommentsDbContext context)
{
    public async Task<Result<Guid>> Handle(
        CreateCommentCommand command, 
        CancellationToken ct)
    {
        var request = command.Request;
        
        var postExists = await context.KnownPosts
            .AnyAsync(x => 
                x.PostId == request.PostId, ct);

        if (!postExists)
            return Result<Guid>.Failure(
                CommentErrors.PostNotFound);

        var comment = new Comment
        {
            Id = Guid.CreateVersion7(),
            PostId = request.PostId,
            AuthorId = command.AuthorId,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow
        };

        await context.Comments.AddAsync(
            comment, ct);

        return Result<Guid>.Success(
            comment.Id);
    }
}