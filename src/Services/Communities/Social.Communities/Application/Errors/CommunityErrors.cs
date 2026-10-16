using Social.Shared.ResultType;

namespace Social.Communities.Application.Errors;

public static class CommunityErrors
{
    public static readonly Error NotFound = 
        Error.NotFound(
            "community.not_found",
            "Сообщество не найдено.");
}