using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace MemoriaPiWeb.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogWarning($"--- E-MAIL WÜRDE GESENDET (an {email}) ---");
            _logger.LogWarning($"Betreff: {subject}");
            _logger.LogWarning($"Nachricht: {htmlMessage}");
            _logger.LogWarning($"-----------------------------------------");

            return Task.CompletedTask;
        }
    }
}