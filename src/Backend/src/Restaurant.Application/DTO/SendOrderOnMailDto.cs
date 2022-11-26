namespace Restaurant.Application.DTO
{
    public sealed record SendOrderOnMailDto(Guid OrderId, string email);
}
