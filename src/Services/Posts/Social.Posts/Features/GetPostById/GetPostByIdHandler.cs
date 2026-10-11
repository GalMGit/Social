using Microsoft.EntityFrameworkCore;
using Social.Posts.Application.DTOs;
using Social.Posts.Application.Errors;
using Social.Posts.Application.Media;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;

namespace Social.Posts.Features.GetPostById;

public sealed class GetPostByIdHandler(
    PostsDbContext context,
    IMediaUrlBuilder urlBuilder)
{
    public async Task<Result<PostResponse>> Handle(
        GetPostByIdQuery query, 
        CancellationToken ct)
    {
        var post = await context.Posts
            .AsNoTracking()
            .Where(x => x.Id == query.PostId)
            .Select(x => new PostResponse(
                x.Id,
                x.AuthorId,
                x.Title,
                x.Content,
                x.CommunityId,
                urlBuilder.ToPublicUrl(x.ImagePath),
                context.KnownUsers
                    .Where(u => u.UserId == x.AuthorId)
                    .Select(u => u.Username)
                    .FirstOrDefault(),
                x.CreatedAt))
            .SingleOrDefaultAsync(ct);
        
        if(post is null)
            return Result<PostResponse>.Failure(
                PostErrors.NotFound);

        return Result<PostResponse>.Success(
            post);
    }
}