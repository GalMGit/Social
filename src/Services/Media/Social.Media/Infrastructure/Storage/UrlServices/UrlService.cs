using Social.Media.Application.Abstractions.IServices.IMediaServices;

namespace Social.Media.Infrastructure.Storage.UrlServices;

public sealed class UrlService(
    IPublicStorage publicStorage) 
    : IMediaUrlService
{
    public string? GetUrl(string? relativePath)
        => string.IsNullOrWhiteSpace(relativePath) 
            ? null 
            : publicStorage.GetPublicUrl(relativePath);

    public string? GetThumbnailUrl(string? relativePath)
        => string.IsNullOrWhiteSpace(relativePath) 
            ? null 
            : publicStorage.GetPublicUrl(relativePath);
}