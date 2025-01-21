namespace Project.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendTwoFactorCode(string email, string code);
    }
}
