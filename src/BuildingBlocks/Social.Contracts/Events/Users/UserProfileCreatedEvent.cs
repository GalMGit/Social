namespace Social.Contracts.Events.Users;

public sealed record UserProfileCreatedEvent(
    Guid UserId, 
    string Username);