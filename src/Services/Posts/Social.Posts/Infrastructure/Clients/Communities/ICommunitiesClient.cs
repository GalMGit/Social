using Social.Contracts.Http.Communities;
using Social.Shared.ResultType;

namespace Social.Posts.Infrastructure.Clients.Communities;

public interface ICommunitiesClient
{
    Task<Result<PostingPermissionResponse>> CheckPostingPermissionAsync(
        Guid communityId,
        string accessToken,
        CancellationToken ct);
}