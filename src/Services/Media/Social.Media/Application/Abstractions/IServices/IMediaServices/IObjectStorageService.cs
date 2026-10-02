namespace Social.Media.Application.Abstractions.IServices.IMediaServices;

public interface IObjectStorage
{
    Task UploadAsync(
        string key, 
        Stream stream,
        string contentType,
        CancellationToken ct = default);

    Task<Stream> DownloadAsync(
        string key,
        CancellationToken ct = default);

    Task DeleteAsync(
        string key,
        CancellationToken ct = default);

    Task DeletePrefixAsync(
        string prefix,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default);

    string GetPublicUrl(string key);
}