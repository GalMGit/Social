using Social.Shared.ResultType;

namespace Social.Posts.Application.Errors;

public static class PostErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "post.not_found",
            "Пост не найден.");

    public static readonly Error TitleTooLong =
        Error.Validation(
            "post.title_too_long",
            "Заголовок не может быть больше 200 символов.");

    public static readonly Error ContentTooLong =
        Error.Validation(
            "post.content_too_long",
            "Содержимое не может быть больше 10000 символов.");
}
