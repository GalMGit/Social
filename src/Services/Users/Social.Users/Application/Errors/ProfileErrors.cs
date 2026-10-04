using Social.Shared.ResultType;

namespace Social.Users.Application.Errors;

public static class ProfileErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "profile.not_found",
            "Профиль не найден.");
}