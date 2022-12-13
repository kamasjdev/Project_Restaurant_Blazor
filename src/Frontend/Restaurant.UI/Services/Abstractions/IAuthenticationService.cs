using Restaurant.UI.DTO;

namespace Restaurant.UI.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<AuthDto> SignInAsync(SignInDto signInDto);
        Task SignUpAsync(SignUpDto signUpDto);
        Task SignoutAsync();
    }
}
