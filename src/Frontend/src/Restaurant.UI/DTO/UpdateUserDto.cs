namespace Restaurant.UI.DTO
{
    public record UpdateUserDto(Guid UserId, string Email, string Password, string Role);
}
