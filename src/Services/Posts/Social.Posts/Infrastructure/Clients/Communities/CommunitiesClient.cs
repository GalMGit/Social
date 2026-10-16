using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Social.Contracts.Http.Common;
using Social.Contracts.Http.Communities;
using Social.Shared.ResultType;

namespace Social.Posts.Infrastructure.Clients.Communities;

public sealed class CommunitiesClient(
    HttpClient httpClient,
    ILogger<CommunitiesClient> logger) : ICommunitiesClient
{
    public async Task<Result<PostingPermissionResponse>> CheckPostingPermissionAsync(
        Guid communityId,
        string accessToken,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/v1/internal/communities/{communityId}/posting-permission");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response;

        try
        {
            response = await httpClient.SendAsync(request, ct);
        }
        catch (HttpRequestException) when (!ct.IsCancellationRequested)
        {
            logger.LogWarning(
                "Сервис Communities недоступен. Не удалось проверить права на создание поста для сообщества {CommunityId}.",
                communityId);

            return Result<PostingPermissionResponse>.Failure(
                Error.ServiceUnavailable(
                    "community.service_unavailable",
                    "Сервис сообществ временно недоступен."));
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);

                return Result<PostingPermissionResponse>.Failure(
                    Error.NotFound(
                        error?.Code ?? "community.not_found",
                        error?.Message ?? "Сообщество не найдено."));
            }

            response.EnsureSuccessStatusCode();

            var permission = await response.Content
                .ReadFromJsonAsync<PostingPermissionResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException(
                    "Communities Service returned an empty response.");

            return Result<PostingPermissionResponse>.Success(permission);
        }
    }
}