using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using static Restaurant.Application.Mail.EmailMessage;

namespace Restaurant.Application.Services
{
    internal sealed class MailService : IMailService
    {
        private readonly IMailSender _mailSender;
        private readonly IOrderRepository _orderRepository;

        public MailService(IMailSender mailSender, IOrderRepository orderRepository)
        {
            _mailSender = mailSender;
            _orderRepository = orderRepository;
        }

        public async Task SendOrderOnMail(SendOrderOnMailDto sendMailDto)
        {
            var order = await _orderRepository.GetAsync(sendMailDto.OrderId);

            if (order is null)
            {
                throw new OrderNotFoundException(sendMailDto.OrderId);
            }

            var content = new EmailMessageBuilder().ConstructEmailFromOrder(order);
            await _mailSender.SendAsync(Email.Of(sendMailDto.email), content);
        }
    }
}
