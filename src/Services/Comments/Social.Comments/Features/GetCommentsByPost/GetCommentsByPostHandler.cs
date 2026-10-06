using Microsoft.EntityFrameworkCore;
using Social.Comments.Application.DTOs;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;

namespace Social.Comments.Features.GetCommentsByPost;

public sealed class GetCommentsByPostHandler(
    CommentsDbContext context)
{
    public async Task<Result<List<CommentResponse>>> Handle(
        GetCommentsByPostQuery query,
        CancellationToken ct)
    {
        var comments = await context.Comments
            .AsNoTracking()
            .Where(x => x.PostId == query.PostId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CommentResponse(
                x.Id,
                x.PostId,
                x.Text,
                x.AuthorId,
                context.KnownUsers
                    .Where(u => u.UserId == x.AuthorId)
                    .Select(u => u.Username)
                    .FirstOrDefault() ?? "Unknown",
                x.CreatedAt))
            .ToListAsync(ct);

        return Result<List<CommentResponse>>.Success(
            comments);
    }
}