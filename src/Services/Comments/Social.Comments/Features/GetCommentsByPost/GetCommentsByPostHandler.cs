using Microsoft.EntityFrameworkCore;
using Social.Comments.Application.DTOs;
using Social.Comments.Application.Mappers;
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
            .Where(x =>
                x.PostId == query.PostId)
            .OrderByDescending(x =>
                x.CreatedAt)
            .ToListAsync(ct);

        return Result<List<CommentResponse>>.Success(
            comments
                .Select(x => 
                    x.ToCommentResponse())
                .ToList());
    }
}