using Restaurant.Application.Mail;
using System.Net.Mail;

namespace Restaurant.UnitTests.Stubs
{
    internal sealed class SmtpClientStub : ISmtpClient
    {
        private readonly int _timeout = 0;
        private readonly List<MailMessage> _messages = new();

        public IEnumerable<MailMessage> Messages => _messages;

        public SmtpClientStub() { }

        public SmtpClientStub(int timeout)
        {
            _timeout = timeout;
        }

        public Task SendMailAsync(MailMessage mailMessage)
        {
            Thread.Sleep(_timeout);
            _messages.Add(mailMessage);
            return Task.CompletedTask;
        }
    }
}
