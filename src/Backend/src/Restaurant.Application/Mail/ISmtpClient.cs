using System.Net.Mail;

namespace Restaurant.Application.Mail
{
    internal interface ISmtpClient
    {
        Task SendMailAsync(MailMessage mailMessage);
    }
}
