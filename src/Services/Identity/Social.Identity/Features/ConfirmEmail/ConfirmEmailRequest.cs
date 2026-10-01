namespace Social.Identity.Features.ConfirmEmail;

public sealed record ConfirmEmailRequest(
    string Email, 
    string Code);