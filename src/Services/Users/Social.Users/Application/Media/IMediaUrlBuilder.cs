namespace Social.Users.Application.Media;

public interface IMediaUrlBuilder
{
    string? ToPublicUrl(string? imagePath);
}