using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Mail;
using Restaurant.Core.Entities;
using Restaurant.Core.ValueObjects;
using Restaurant.UnitTests.Stubs;
using static Restaurant.Application.Mail.EmailMessage;
using System.Net.Mail;
using Shouldly;

namespace Restaurant.UnitTests.Mails
{
    public class MailSenderTests
    {
        [Fact]
        public async Task should_send_email()
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true, Login = "login", Email = email.Value, Password = "password", SmtpClient = "smtpClient", SmtpPort = 100, Timeout = 100 });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldBeNull();
            await _smtpClient.Received(1).SendMailAsync(Arg.Any<MailMessage>());
        }

        [Fact]
        public async Task given_options_disabled_mail_shouldnt_send()
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = false });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);

            await mailSender.SendAsync(email, mailMessage);

            await _smtpClient.DidNotReceiveWithAnyArgs().SendMailAsync(Arg.Any<MailMessage>());
        }

        [Fact]
        public async Task given_invalid_login_should_throw_an_exception()
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true, Email = "email@email.com", Password = "PAsW", SmtpClient = "test", SmtpPort = 12, Timeout = 10 });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);
            var error = new InvalidEmailSettings();
            error.AddProperty(nameof(EmailSettings.Login));
            var expectedException = new InvalidEmailSettingsException(error);

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((InvalidEmailSettingsException)exception).InvalidEmailSettings.ToString().ShouldBe(expectedException.InvalidEmailSettings.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("abc")]
        [InlineData("abc1234124@")]
        public async Task given_empty_email_should_throw_an_exception(string emailSettings)
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true, Login = "login", Email = emailSettings, Password = "password", SmtpClient = "test", SmtpPort = 100 });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);
            var error = new InvalidEmailSettings();
            var expectedException = new InvalidEmailSettingsException(error);

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldContain(expectedException.Message);
            ((InvalidEmailSettingsException)exception).InvalidEmailSettings.HasErrors().ShouldBeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task given_empty_password_should_throw_an_exception(string password)
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true, Login = "login", Email = email.Value, Password = password, SmtpClient = "test", SmtpPort = 100 });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);
            var expectedException = new InvalidEmailSettingsException(new InvalidEmailSettings());

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            ((InvalidEmailSettingsException)exception).InvalidEmailSettings.HasErrors().ShouldBeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task given_empty_smtp_client_should_throw_an_exception(string smtpClient)
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true, Login = "login", Email = email.Value, Password = "password", SmtpClient = smtpClient, SmtpPort = 100, Timeout = 100 });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);
            var error = new InvalidEmailSettings();
            error.AddProperty(nameof(EmailSettings.SmtpClient));
            var expectedException = new InvalidEmailSettingsException(error);

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldContain(expectedException.Message);
            ((InvalidEmailSettingsException)exception).InvalidEmailSettings.ToString().ShouldBe(expectedException.InvalidEmailSettings.ToString());
        }

        [Fact]
        public async Task given_invalid_options_with_enabled_email_send_should_throw_an_exception_for_all()
        {
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var mailSender = new MailSender(_emailSettings, _logger, _smtpClient);

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldBeOfType<InvalidEmailSettingsException>();
            ((InvalidEmailSettingsException)exception).InvalidEmailSettings.HasErrors().ShouldBeTrue();
            var errorMessage = ((InvalidEmailSettingsException)exception).InvalidEmailSettings.ToString();
            errorMessage.ShouldContain(nameof(EmailSettings.Email));
            errorMessage.ShouldContain(nameof(EmailSettings.Login));
            errorMessage.ShouldContain(nameof(EmailSettings.Password));
            errorMessage.ShouldContain(nameof(EmailSettings.SmtpClient));
            errorMessage.ShouldContain(nameof(EmailSettings.SmtpPort));
            errorMessage.ShouldContain(nameof(EmailSettings.Timeout));
        }

        [Fact]
        public async Task cannot_send_email_timeout_exceeded()
        {
            var timeout = 1;
            var email = Email.Of("email@test.com");
            _emailSettings.CurrentValue.Returns(new EmailSettings { SendEmailAfterConfirmOrder = true, Login = "login", Email = email.Value, Password = "password", SmtpClient = "smtpClient", SmtpPort = 100,
                Timeout = timeout });
            var mailMessage = _emailMessageBuilder.ConstructEmailFromOrder(CreateDefaultOrderWithPositions());
            var smtpClient = new SmtpClientStub(timeout + 20);
            var expectedException = new CannotSendEmailException();
            var mailSender = new MailSender(_emailSettings, _logger, smtpClient);

            var exception = await Record.ExceptionAsync(() => mailSender.SendAsync(email, mailMessage));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldContain(expectedException.Message);
        }

        private Order CreateDefaultOrderWithPositions()
        {
            var productSales = new List<ProductSale> { CreateDefaultProductSale(), CreateDefaultProductSale() };
            var order = new Order(Guid.NewGuid(), "ORDER", _currentDate, productSales.Sum(p => p.EndPrice), Email.Of("email@email.com"), products: productSales);
            return order;
        }

        private ProductSale CreateDefaultProductSale()
        {
            var product = CreateDefaultProduct();
            var productSale = new ProductSale(Guid.NewGuid(), product, ProductSaleState.Ordered, Email.Of("email@email.com"));
            return productSale;
        }

        private Product CreateDefaultProduct()
        {
            var product = new Product(Guid.NewGuid(), Guid.NewGuid().ToString("N"), 10, ProductKind.MainDish);
            return product;
        }

        private readonly DateTime _currentDate;
        private readonly ILogger<MailSender> _logger;
        private readonly IOptionsMonitor<EmailSettings> _emailSettings;
        private readonly EmailMessageBuilder _emailMessageBuilder;
        private readonly ISmtpClient _smtpClient;


        public MailSenderTests()
        {
            _currentDate = new DateTime(2022, 9, 13, 18, 20, 30);
            _emailSettings = Substitute.For<IOptionsMonitor<EmailSettings>>();
            _logger = Substitute.For<ILogger<MailSender>>();
            _emailMessageBuilder = new EmailMessageBuilder();
            _smtpClient = Substitute.For<ISmtpClient>();
        }
    }
}
