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
            .SingleOrDefaultAsync(x => 
                x.Id == query.Id, ct);
        
        if(comment is null)
            return Result<CommentResponse>.Failure(
                CommentErrors.NotFound);

        return Result<CommentResponse>.Success(
            comment.ToCommentResponse());
    }
}