namespace Social.Identity.Features.Login;

public sealed record LoginRequest(
    string Email, 
    string Password);