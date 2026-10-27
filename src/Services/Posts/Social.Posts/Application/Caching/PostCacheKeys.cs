namespace Social.Posts.Application.Caching;

public static class PostCacheKeys
{
    public const string All = "posts:all";

    public static string ById(Guid postId) => $"post:{postId}";
}