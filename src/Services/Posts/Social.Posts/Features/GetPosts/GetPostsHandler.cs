using Microsoft.EntityFrameworkCore;
using Social.Posts.Application.DTOs;
using Social.Posts.Application.Media;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;

namespace Social.Posts.Features.GetPosts;

public sealed class GetPostsHandler(
    PostsDbContext context,
    IMediaUrlBuilder urlBuilder)
{
    public async Task<Result<List<PostResponse>>> Handle(
        GetPostsQuery query,
        CancellationToken ct)
    {
        var posts = await context.Posts
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
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
            .ToListAsync(ct);
        
        return Result<List<PostResponse>>.Success(
            posts);
    }
}