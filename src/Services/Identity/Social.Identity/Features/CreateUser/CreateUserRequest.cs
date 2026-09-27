namespace Social.Identity.Features.CreateUser;

public sealed record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword);