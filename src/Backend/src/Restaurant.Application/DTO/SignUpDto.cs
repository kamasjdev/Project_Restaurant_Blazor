namespace Restaurant.Application.DTO
{
    public record SignUpDto(string Email, string Password, string Role = null);
}
