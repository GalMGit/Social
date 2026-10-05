using Social.Shared.ResultType;

namespace Social.Comments.Application.Errors;

public static class CommentErrors
{
    public static readonly Error PostNotFound = 
        Error.NotFound(
            "comments.post_not_found",
            "Пост с таким Id не найден.");
    
    public static readonly Error NotFound = 
        Error.NotFound(
            "comments.not_found",
            "Комментарий с таким Id не найден.");
}