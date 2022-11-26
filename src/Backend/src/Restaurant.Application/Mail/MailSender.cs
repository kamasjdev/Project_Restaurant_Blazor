using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Restaurant.Application.Abstractions;
using Restaurant.Application.Exceptions;
using Restaurant.Core.ValueObjects;
using System;
using System.Net.Mail;

namespace Restaurant.Application.Mail
{
    internal sealed class MailSender : IMailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<MailSender> _logger;
        private readonly ISmtpClient _smtpClient;

        public MailSender(IOptionsMonitor<EmailSettings> settings, ILogger<MailSender> logger, ISmtpClient smtpClient)
        {
            _settings = settings.CurrentValue;
            _logger = logger;
            _smtpClient = smtpClient;
        }

        public async Task SendAsync(Email email, IEmailMessage emailMessage)
        {
            if (!_settings.SendEmailAfterConfirmOrder)
            {
                _logger.LogInformation("MailSender Service is disabled");
                return;
            }

            ValidateSettings();
            var mailMessage = new MailMessage(_settings.Email, email.Value)
            {
                Subject = emailMessage.Subject,
                Body = emailMessage.Body
            };

            var cancellationTokenSource = new CancellationTokenSource(_settings.Timeout);
            cancellationTokenSource.CancelAfter(_settings.Timeout);
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                var source = new CancellationTokenSource();
                source.CancelAfter(TimeSpan.FromMilliseconds(_settings.Timeout));
                var token = source.Token;
                var tcs = new TaskCompletionSource<bool>();
                cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
                var task = _smtpClient.SendMailAsync(mailMessage);
                await Task.WhenAny(task, tcs.Task);
                token.ThrowIfCancellationRequested();
                await task;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw new CannotSendEmailException();
            }
        }

        private void ValidateSettings()
        {
            var errors = new InvalidEmailSettings();

            if (string.IsNullOrWhiteSpace(_settings.Login))
            {
                errors.AddProperty(nameof(EmailSettings.Login));
            }

            if (string.IsNullOrWhiteSpace(_settings.Email))
            {
                errors.AddProperty(nameof(EmailSettings.Email));
            }

            if (string.IsNullOrWhiteSpace(_settings.Password))
            {
                errors.AddProperty(nameof(EmailSettings.Password));
            }

            if (string.IsNullOrWhiteSpace(_settings.SmtpClient))
            {
                errors.AddProperty(nameof(EmailSettings.SmtpClient));
            }

            if (_settings.SmtpPort == default)
            {
                errors.AddProperty(nameof(EmailSettings.SmtpPort));
            }

            if (_settings.Timeout == default)
            {
                errors.AddProperty(nameof(EmailSettings.Timeout));
            }

            try
            {
                Email.Of(_settings.Email);
            }
            catch (Exception exception)
            {
                errors.AddPropertyWithValue(nameof(EmailSettings.Email), exception.Message);
            }

            if (errors.HasErrors())
            {
                throw new InvalidEmailSettingsException(errors);
            }
        }
    }
}
