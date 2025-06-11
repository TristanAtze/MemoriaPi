using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MemoriaPiWeb.Services
{
    // Diese Klasse wird unsere Konfigurationseinstellungen speichern
    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // Wir holen den API-Schlüssel aus dem Secret Manager
            var apiKey = _configuration["SendGridKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("SendGridKey ist nicht konfiguriert.");
                throw new Exception("SendGridKey not configured.");
            }

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                // Hier Ihre verifizierte Absender-E-Mail und Ihren Namen eintragen
                From = new EmailAddress("tristan.atze@gmail.com", "MemoriaPi Support"),
                Subject = subject,
                PlainTextContent = message, // SendGrid benötigt Plain-Text, auch wenn wir HTML senden
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // E-Mail senden
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email to {toEmail} queued successfully!", toEmail);
            }
            else
            {
                _logger.LogError("Failed to send email to {toEmail}. Status Code: {statusCode}", toEmail, response.StatusCode);
            }
        }
    }
}