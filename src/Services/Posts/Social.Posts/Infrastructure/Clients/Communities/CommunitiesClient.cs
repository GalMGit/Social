using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Social.Contracts.Http.Common;
using Social.Contracts.Http.Communities;
using Social.Shared.ResultType;

namespace Social.Posts.Infrastructure.Clients.Communities;

public sealed class CommunitiesClient(HttpClient httpClient)
    : ICommunitiesClient
{
    public async Task<Result<PostingPermissionResponse>>
        CheckPostingPermissionAsync(
            Guid communityId,
            string accessToken,
            CancellationToken ct)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/v1/internal/communities/{communityId}/posting-permission");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var errorResponse =
                await response.Content.ReadFromJsonAsync<ErrorResponse>(
                    cancellationToken: ct);

            if (errorResponse is null)
            {
                return Result<PostingPermissionResponse>.Failure(
                    Error.NotFound(
                        "community.not_found",
                        "Сообщество не найден."));
            }

            return Result<PostingPermissionResponse>.Failure(
                Error.NotFound(
                    errorResponse.Code,
                    errorResponse.Message));
        }

        response.EnsureSuccessStatusCode();

        var permission =
            await response.Content.ReadFromJsonAsync<PostingPermissionResponse>(
                cancellationToken: ct)
            ?? throw new InvalidOperationException(
                "Communities Service returned an empty response.");

        return Result<PostingPermissionResponse>.Success(permission);
    }
}