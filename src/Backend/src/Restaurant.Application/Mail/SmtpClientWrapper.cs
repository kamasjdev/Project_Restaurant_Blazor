using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace Restaurant.Application.Mail
{
    internal sealed class SmtpClientWrapper : ISmtpClient
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailSettings _emailSettings;

        public SmtpClientWrapper(IOptionsMonitor<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.CurrentValue;
            _smtpClient = new SmtpClient(_emailSettings.SmtpClient, _emailSettings.SmtpPort)
            {
                Timeout = _emailSettings.Timeout,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Login, _emailSettings.Password),
                EnableSsl = true
            };
        }

        public async Task SendMailAsync(MailMessage mailMessage)
        {
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
