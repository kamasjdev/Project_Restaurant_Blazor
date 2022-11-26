namespace Restaurant.Application.DTO
{
    public record UpdateUserDto(Guid UserId, string Email, string Password, string Role);
}
