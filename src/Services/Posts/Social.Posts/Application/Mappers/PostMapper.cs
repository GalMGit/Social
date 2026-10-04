using Social.Posts.Application.DTOs;
using Social.Posts.Domain;

namespace Social.Posts.Application.Mappers;

public static class PostMapper
{
    public static PostResponse ToPostResponse(
        this Post post,
        string? imageUrl)
    {
        return new PostResponse(
            post.Id,
            post.AuthorId,
            post.Title,
            post.Content,
            post.CommunityId,
            imageUrl,
            post.CreatedAt);
    }
}