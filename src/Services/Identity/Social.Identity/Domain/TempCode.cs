namespace Social.Identity.Domain;

public sealed class TempCode
{
    public string ConfirmationCode { get; set; }
    public DateTime CodeExpiresAt { get; set; }
}