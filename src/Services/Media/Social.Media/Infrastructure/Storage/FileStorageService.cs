using MimeDetective;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Social.Media.Application.Abstractions.IServices.IMediaServices;

namespace Social.Media.Infrastructure.Storage;

public sealed class FileStorageService : IFileStorageService
{
    private readonly IPublicStorage _publicStorage;
    private readonly IContentInspector _inspector;

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png"
    ];

    public FileStorageService(
        IPublicStorage publicStorage)
    {
        _publicStorage = publicStorage;
        
        _inspector = new ContentInspectorBuilder
        {
            Definitions =
            [
                .. MimeDetective.Definitions.DefaultDefinitions.FileTypes.Images.JPEG(),
                .. MimeDetective.Definitions.DefaultDefinitions.FileTypes.Images.PNG(),
            ]
        }.Build();
    }

    public string GetUrl(string key)
        => _publicStorage.GetPublicUrl(key);

    public async Task<FileUploadResult> UploadPictureAsync(
        UploadFile file,
        Guid productId,
        CancellationToken ct = default)
    {
        var detectedContentType = Validate(file.Stream);
        ValidateFileSize(file.Length, detectedContentType);

        var mediaKey = CreateMediaKey(
            productId, 
            detectedContentType);

        if (detectedContentType.StartsWith("image/"))
            return await UploadImageWithThumbnailAsync(
                file, 
                productId, 
                mediaKey, 
                detectedContentType, ct);

        throw new Exception(
            $"Unsupported content type: {detectedContentType}");
    }
    
    private static string CreateMediaKey(
        Guid productId, 
        string contentType)
        => $"products/{productId}/{Guid.NewGuid()}{GetExtension(contentType)}";
    
    private static string CreateThumbnailKey(Guid productId)
        => $"products/{productId}/{Guid.NewGuid()}_thumb.jpg";
    
    private static string GetExtension(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            _ => throw new Exception(
                $"Unsupported content type: {contentType}")
        };
    }
    
    private async Task<FileUploadResult> UploadImageWithThumbnailAsync(
        UploadFile file,
        Guid chatId,
        string mediaKey,
        string contentType,
        CancellationToken ct)
    {
        await using var imageStream = await file.Stream.ToMemoryStreamAsync(ct);
        
        await _publicStorage.UploadAsync(
            mediaKey,
            imageStream,
            contentType, ct);

        string? thumbnailKey = null;
        
        imageStream.Position = 0;
        await using var thumbnail = await ProcessImageAsync(
            imageStream,
            StorageConstants.ThumbnailWidth,
            StorageConstants.ThumbnailHeight,
            StorageConstants.ThumbnailQuality, ct);

        thumbnailKey = CreateThumbnailKey(chatId);
            
        await _publicStorage.UploadAsync(
            thumbnailKey, 
            thumbnail, 
            "image/jpeg", ct);

        return new FileUploadResult(
            mediaKey, 
            thumbnailKey, 
            contentType);
    }
    
    private async Task<MemoryStream> ProcessImageAsync(
        Stream stream,
        int width,
        int height,
        int quality,
        CancellationToken ct)
    {
        stream.Position = 0;
        using var image = await Image.LoadAsync(stream, ct);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop
        }));

        var output = new MemoryStream();
        
        await image.SaveAsJpegAsync(
            output, 
            new JpegEncoder { Quality = quality }, ct);
        
        output.Position = 0;

        return output;
    }
    
    private void ValidateFileSize(
        long fileLength, 
        int maxSize)
    {
        if (fileLength > maxSize)
            throw new Exception(
                $"Maximum size is {maxSize / 1024 / 1024} MB.");
    }
    
    private void ValidateFileSize(
        long fileLength, 
        string contentType)
    {
        var maxSize = StorageConstants.MaxImageSize;

        ValidateFileSize(fileLength, maxSize);
    }
    
    private string Validate(Stream stream)
    {
        var matches = _inspector.Inspect(stream);
        stream.Position = 0;

        if (matches.Length == 0)
            throw new Exception(
                "Invalid file type.");

        var mimeType = matches
            .OrderByDescending(m => m.Points)
            .First()
            .Definition.File.MimeType;

        if (mimeType is null || !AllowedContentTypes.Contains(mimeType))
            throw new Exception(
                $"Unsupported content type: {mimeType ?? "unknown"}");

        return mimeType;
    }
}

internal static class StorageConstants
{
    public const int MaxImageSize = 6 * 1024 * 1024;
    
    public const int ThumbnailWidth = 200;
    public const int ThumbnailHeight = 200;
    public const int ThumbnailQuality = 45;
}

internal static class StreamExtensions
{
    public static async Task<MemoryStream> ToMemoryStreamAsync(
        this Stream stream,
        CancellationToken ct = default)
    {
        var memoryStream = new MemoryStream();
        stream.Position = 0;
        await stream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;
        return memoryStream;
    }
}