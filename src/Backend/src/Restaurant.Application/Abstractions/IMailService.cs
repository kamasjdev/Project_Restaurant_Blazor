using Restaurant.Application.DTO;

namespace Restaurant.Application.Abstractions
{
    public interface IMailService
    {
        Task SendOrderOnMail(SendOrderOnMailDto sendMailDto);
    }
}
