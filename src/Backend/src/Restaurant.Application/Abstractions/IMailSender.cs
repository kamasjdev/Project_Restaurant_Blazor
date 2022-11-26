using Restaurant.Application.Mail;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Application.Abstractions
{
    public interface IMailSender
    {
        Task SendAsync(Email email, IEmailMessage emailMessage);
    }
}
