using System.Reflection;
using Social.EmailWorker.Abstractions;

namespace Social.EmailWorker.Email;

public sealed class EmailTemplateService : IEmailTemplateService
{
    public EmailResult GetRegistrationConfirmation(
        string toEmail,
        string username,
        string code)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(
            "Social.EmailWorker.Email.Templates.Register.html");

        if (stream is null)
            throw new FileNotFoundException(
                "Embedded resource Register.html not found.");

        using var reader = new StreamReader(stream);

        var html = reader.ReadToEnd()
            .Replace("{{Username}}", username)
            .Replace("{{Code}}", code);

        return new EmailResult(
            toEmail,
            "Подтверждение регистрации в Social",
            html);
    }
}