namespace Restaurant.Application.DTO
{
    public record ChangePasswordDto(Guid UserId, string Password, string NewPassword, string NewPasswordConfirm);
}
