using Social.EmailWorker.Email;

namespace Social.EmailWorker.Abstractions;

public interface IEmailTemplateService
{
    EmailResult GetRegistrationConfirmation(string toEmail, string username, string code);
}