namespace Social.Media.Application.Abstractions.IServices.IMediaServices;

public interface IFileStorageService
{
    string GetUrl(string key);
    
    Task<FileUploadResult> UploadPictureAsync(
        UploadFile file, 
        Guid productId,
        CancellationToken ct = default);
}

public sealed record FileUploadResult(
    string RelativePath,
    string? ThumbnailRelativePath,
    string? ContentType);
    
public sealed class UploadFile : IAsyncDisposable
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required long Length { get; init; }
    public required Stream Stream { get; init; }
    
    public ValueTask DisposeAsync()
        => Stream.DisposeAsync();
}