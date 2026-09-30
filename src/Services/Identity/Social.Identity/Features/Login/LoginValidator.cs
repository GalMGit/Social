using System.Text.RegularExpressions;
using FluentValidation;

namespace Social.Identity.Features.Login;

public class LoginUserValidator : AbstractValidator<LoginRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым")
            .MaximumLength(100).WithMessage("Email не может быть больше 100 символов")
            .EmailAddress().WithMessage("Неверный формат email")
            .Must(email =>
                email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) ||
                email.EndsWith("@yandex.ru", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Разрешены только Gmail и Yandex");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .MinimumLength(5).WithMessage("Пароль должен содержать не менее 5 символов")
            .MaximumLength(40).WithMessage("Пароль должен содержать не более 40 символов")
            .Must(password => Regex.IsMatch(password, @"[A-Za-z]") && Regex.IsMatch(password, @"\d"))
            .WithMessage("Пароль должен содержать хотя бы одну букву и одну цифру");
    }
}