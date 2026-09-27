namespace Social.Contracts.Events.Identity;

public sealed record UserStartRegistrationEvent(
    string Email, 
    string Username,
    string ConfirmationCode);