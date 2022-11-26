using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Restaurant.Application.Abstractions;
using Restaurant.Application.Mail;
using Restaurant.Core.Entities;
using Restaurant.Core.ValueObjects;
using Restaurant.UnitTests.Stubs;
using Shouldly;
using static Restaurant.Application.Mail.EmailMessage;

namespace Restaurant.UnitTests.Mails
{
    public class MailSendFlowTests
    {
        [Fact]
        public async Task should_send_email()
        {
            var order = CreateDefaultOrderWithPositions();
            var mail = _emailMessageBuilder.ConstructEmailFromOrder(order);
            var emailTo = Email.Of("test@test.com");

            await _mailSender.SendAsync(emailTo, mail);

            var messages = _smtpClient.Messages.ToList();
            var mailSent = messages.Where(m => m.To.Any(m => m.Address == emailTo.Value)).SingleOrDefault();
            mailSent.ShouldNotBeNull();
            mailSent.Body.ShouldContain(order.OrderNumber);
            mailSent.Body.ShouldContain(order.Price.Value.ToString());
        }

        private Order CreateDefaultOrderWithPositions()
        {
            var productSales = new List<ProductSale> { CreateDefaultProductSale(), CreateDefaultProductSale() };
            var order = new Order(Guid.NewGuid(), "ORDER", _clock.CurrentDate(), productSales.Sum(p => p.EndPrice), Email.Of("email@email.com"), products: productSales);
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

        private readonly IMailSender _mailSender;
        private readonly IOptionsMonitor<EmailSettings> _emailSettings;
        private readonly ILogger<MailSender> _logger;
        private readonly SmtpClientStub _smtpClient;
        private readonly IClock _clock;
        private readonly EmailMessageBuilder _emailMessageBuilder;

        public MailSendFlowTests()
        {
            _emailSettings = Substitute.For<IOptionsMonitor<EmailSettings>>();
            _emailSettings.CurrentValue.Returns(new EmailSettings
            {
                SendEmailAfterConfirmOrder = true,
                Email = "email@test.com",
                Login = "login",
                Password = "password",
                SmtpClient = "smtp",
                SmtpPort = 10,
                Timeout = 1000
            });
            _logger = Substitute.For<ILogger<MailSender>>();
            _smtpClient = new SmtpClientStub();
            _clock = Substitute.For<IClock>();
            _clock.CurrentDate().Returns(new DateTime(2022, 9, 30, 15, 35, 10));
            _emailMessageBuilder = new EmailMessageBuilder();
            _mailSender = new MailSender(_emailSettings, _logger, _smtpClient);
        }
    }
}
