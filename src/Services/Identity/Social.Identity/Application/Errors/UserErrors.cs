using Social.Shared.ResultType;

namespace Social.Identity.Application.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "user.not_found", 
            "Пользователь не найден.");
    
    public static readonly Error EmailExists =
        Error.NotFound(
            "user.email_exists",    
            "Этот Email занят.");
    
    public static readonly Error ConfirmationCodeNotFound =
        Error.NotFound(
            "user.confirmation_code_not_found", 
            "Код подтверждения не найден, зарегистрируйтесь заново.");
    
    public static readonly Error ConfirmationCodeInvalid =
        Error.Validation(
            "user.confirmation_code_invalid", 
            "Неверный код подтверждения.");
    
    public static readonly Error ConfirmationCodeExpired =
        Error.Validation(
            "user.confirmation_code_expired", 
            "Срок действия кода истек, зарегистрируйтесь заново.");
    
    public static readonly Error RegistrationPending =
        Error.Conflict(
            "user.registration_pending",
            "На этот email уже отправлен код подтверждения. Повторите попытку через 5 минут.");
}