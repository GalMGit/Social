using Social.Contracts.Events.Posts;
using Social.Media.Application.Abstractions.IServices.IMediaServices;

namespace Social.Media.IntegrationEvents.Posts;

public sealed class PostDeletedHandler(
    IFileStorageService storage)
{
    public async Task Handle(
        PostDeletedEvent @event,
        CancellationToken ct)
        => await storage.DeletePostMediaAsync(
            @event.PostId,
            ct);
}