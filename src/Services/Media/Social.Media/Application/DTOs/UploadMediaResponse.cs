namespace Social.Media.Application.DTOs;

public sealed record UploadMediaResponse(
    string ImagePath,
    string? ThumbnailPath);