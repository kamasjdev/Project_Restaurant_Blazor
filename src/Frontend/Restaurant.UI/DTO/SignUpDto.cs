namespace Restaurant.UI.DTO
{
    public record SignUpDto(string Email, string Password, string Role = null);
}
