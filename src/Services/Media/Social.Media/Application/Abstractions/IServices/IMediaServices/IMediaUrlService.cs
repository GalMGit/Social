namespace Social.Media.Application.Abstractions.IServices.IMediaServices;

public interface IMediaUrlService
{
    string? GetUrl(string? relativePath);
    string? GetThumbnailUrl(string? relativePath);
}