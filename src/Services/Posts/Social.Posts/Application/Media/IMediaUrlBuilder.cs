namespace Social.Posts.Application.Media;

public interface IMediaUrlBuilder
{
    string? ToPublicUrl(string? imagePath);
}