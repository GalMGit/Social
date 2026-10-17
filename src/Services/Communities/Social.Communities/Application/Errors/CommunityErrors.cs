using Social.Shared.ResultType;

namespace Social.Communities.Application.Errors;

public static class CommunityErrors
{
    public static readonly Error NotFound = 
        Error.NotFound(
            "community.not_found",
            "Сообщество не найдено.");
    
    public static readonly Error AccessDenied = 
        Error.Forbidden(
            "community.delete_error",
            "У вас нет прав для удаления этого сообщества.");
}