using System.Net.Http.Headers;

namespace Social.Posts.Infrastructure.Authentication;

public sealed class HttpUserAccessToken(
    IHttpContextAccessor httpContextAccessor)
    : IUserAccessToken
{
    public string GetAccessToken()
    {
        var authorization = httpContextAccessor
            .HttpContext?
            .Request.Headers.Authorization
            .ToString();

        if (!AuthenticationHeaderValue.TryParse(
                authorization,
                out var value) ||
            !string.Equals(
                value.Scheme,
                "Bearer",
                StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(value.Parameter))
        {
            throw new InvalidOperationException(
                "Bearer access token is unavailable.");
        }

        return value.Parameter;
    }
}