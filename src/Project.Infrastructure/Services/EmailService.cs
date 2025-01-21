using System.Net;
using Project.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Project.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _sendGridApiKey;

        public EmailService(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("no-reply@yourdomain.com", "Your App Name");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to send email. Status: {response.StatusCode}");
            }
        }

        public async Task SendTwoFactorCode(string email, string code)
        {
            var subject = "Your Two-Factor Authentication Code";
            var htmlContent = $"<p>Your authentication code is: <strong>{code}</strong></p>";

            await SendEmailAsync(email, subject, htmlContent);
        }
    }
}
