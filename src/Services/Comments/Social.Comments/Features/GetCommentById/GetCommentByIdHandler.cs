using Microsoft.EntityFrameworkCore;
using Social.Comments.Application.DTOs;
using Social.Comments.Application.Errors;
using Social.Comments.Application.Mappers;
using Social.Comments.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;

namespace Social.Comments.Features.GetCommentById;

public sealed class GetCommentByIdHandler(
    CommentsDbContext context)
{
    public async Task<Result<CommentResponse>> Handle(
        GetCommentByIdQuery query, 
        CancellationToken ct)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
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
            .SingleOrDefaultAsync(ct);
        
        if(comment is null)
            return Result<CommentResponse>.Failure(
                CommentErrors.NotFound);

        return Result<CommentResponse>.Success(
            comment);
    }
}