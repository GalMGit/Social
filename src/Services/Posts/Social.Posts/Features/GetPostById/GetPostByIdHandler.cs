using Microsoft.EntityFrameworkCore;
using Social.Posts.Application.DTOs;
using Social.Posts.Application.Errors;
using Social.Posts.Application.Mappers;
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
            .SingleOrDefaultAsync(x => 
                x.Id == query.PostId, ct);
        
        if(post is null)
            return Result<PostResponse>.Failure(
                PostErrors.NotFound);

        var imageUrl = urlBuilder.ToPublicUrl(
            post.ImagePath);

        return Result<PostResponse>.Success(
            post.ToPostResponse(imageUrl));
    }
}