using Social.EmailWorker.Abstractions;
using Social.EmailWorker.Email;

namespace Social.EmailWorker.DI;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddConfiguration(IConfiguration configuration)
        {
            services.Configure<EmailOptions>(
                configuration.GetSection(nameof(EmailOptions)));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        }
    }
}