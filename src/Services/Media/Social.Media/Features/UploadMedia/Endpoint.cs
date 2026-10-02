using System.Security.Claims;
using Social.Media.Application.Abstractions.IServices.IMediaServices;
using Social.Media.Application.DTOs;
using Social.Shared.Endpoint;

namespace Social.Media.Features.UploadMedia;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("media", async (
                IFormFile file,
                ClaimsPrincipal user,
                IFileStorageService storage,
                CancellationToken ct) =>
            {
                if (file.Length == 0)
                    return Results.BadRequest(
                        new { error = "File is required" });

                var ownerId = user.GetUserId();

                await using var upload = new UploadFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Length = file.Length,
                    Stream = file.OpenReadStream()
                };

                var result = await storage.UploadPictureAsync(
                    upload,
                    ownerId,
                    ct);

                return Results.Ok(new UploadMediaResponse(
                    result.RelativePath,
                    result.ThumbnailRelativePath));
            })
            .RequireAuthorization()
            .WithTags("Media")
            .DisableAntiforgery();
    }
}