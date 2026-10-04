namespace Social.Users.Application.DTOs;

public sealed record ProfileResponse(
    Guid Id,
    string Username,
    string? ImageUrl);