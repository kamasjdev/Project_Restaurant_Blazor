using Blazored.LocalStorage;
using Restaurant.UI.DTO;
using Restaurant.UI.Security;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IUserService _userService;
        private const string TOKEN = "token";

        public AuthenticationService(ILocalStorageService localStorageService, IUserService userService)
        {
            _localStorageService = localStorageService;
            _userService = userService;
        }

        public async Task<UserDto> SignInAsync(SignInDto signInDto)
        {
            var auth = await _userService.SignInAsync(signInDto);
            await _localStorageService.SetItemAsync(TOKEN, auth);
            return JwtExtensions.ParseUserFromJwt(auth.AccessToken!)!;
        }

        public async Task SignoutAsync()
        {
            await _localStorageService.RemoveItemAsync(TOKEN);
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            await _userService.SignUpAsync(signUpDto);
        }
    }    
}
