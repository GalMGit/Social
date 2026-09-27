namespace Social.Contracts.Events.Identity;

public sealed record UserCreatedEvent(
    Guid UserId);