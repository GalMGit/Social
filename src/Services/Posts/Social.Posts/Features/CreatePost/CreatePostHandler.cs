using Social.Contracts.Events.Posts;
using Social.Posts.Application.Errors;
using Social.Posts.Domain;
using Social.Posts.Infrastructure.Authentication;
using Social.Posts.Infrastructure.Clients.Communities;
using Social.Posts.Infrastructure.Persistence.Context;
using Social.Shared.ResultType;
using Wolverine;
using Wolverine.Attributes;

namespace Social.Posts.Features.CreatePost;

[Transactional(typeof(PostsDbContext))]
public sealed class CreatePostHandler(
    PostsDbContext context,
    ICommunitiesClient client,
    IUserAccessToken userAccessToken,
    IMessageBus bus)
{
    public async Task<Result<Guid>> Handle(
        CreatePostCommand command,
        CancellationToken ct)
    {
        var request = command.Request;

        if (request.CommunityId is not null)
        {
            var token = userAccessToken.GetAccessToken();
            
            var result = await client.CheckPostingPermissionAsync(
                request.CommunityId.Value,
                token,
                ct);

            if (result.IsFailure)
                return Result<Guid>.Failure(result.Error!);
            
            if (!result.Value.CanCreatePost)
                return Result<Guid>.Failure(
                    PostErrors.AccessDenied);
        }

        var post = new Post
        {
            Id = Guid.CreateVersion7(),
            AuthorId = command.AuthorId,
            Title = request.Title,
            Content = request.Content,
            ImagePath = command.Request.ImagePath,
            CommunityId = request.CommunityId,
            CreatedAt = DateTime.UtcNow
        };

        await context.Posts.AddAsync(
            post, ct);

        await bus.PublishAsync(
            new PostCreatedEvent(
                post.Id,
                post.AuthorId,
                post.Title,
                post.Content,
                post.CommunityId));

        return Result<Guid>.Success(
            post.Id);
    }
}