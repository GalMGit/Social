using Social.Contracts.Events.Posts;
using Social.Media.Application.Abstractions.IServices.IMediaServices;

namespace Social.Media.IntegrationEvents.Posts;

public sealed class PostDeletedHandler(
    IFileStorageService storage,
    ILogger<PostDeletedHandler> logger)
{
    public async Task Handle(
        PostDeletedEvent @event,
        CancellationToken ct)
    {
        logger.LogInformation(
            "PostDeleted получен. PostId: {PostId}. MediaKeys: {MediaKeys}",
            @event.PostId,
            string.Join(", ", @event.MediaKeys));

        await storage.DeletePostMediaAsync(
            @event.MediaKeys,
            ct);

        logger.LogInformation(
            "Удаление объектов завершено. PostId: {PostId}",
            @event.PostId);
    }
}