using Microsoft.Extensions.Options;

namespace Social.Users.Application.Media;

public sealed class MediaUrlBuilder(
    IOptions<MediaOptions> options) 
    : IMediaUrlBuilder
{
    private readonly string _baseUrl = options.Value.BaseUrl.TrimEnd('/');

    public string? ToPublicUrl(string? imagePath)
        => string.IsNullOrWhiteSpace(imagePath)
            ? null
            : $"{_baseUrl}/{imagePath.TrimStart('/')}";
}