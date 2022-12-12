using Blazored.LocalStorage;
using Restaurant.UI.DTO;
using Restaurant.UI.Extensions;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserService _userService;
        private const string TOKEN = "token";

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userService = serviceProvider.GetRequiredService<IUserService>();
        }

        public async Task<UserDto> SignInAsync(SignInDto signInDto)
        {
            using var scope = _serviceProvider.CreateScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var auth = await _userService.SignInAsync(signInDto);
            await localStorageService.SetItemAsync(TOKEN, auth);
            return JwtExtensions.ParseUserFromJwt(auth.AccessToken!)!;
        }

        public async Task SignoutAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            await localStorageService.RemoveItemAsync(TOKEN);
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            await _userService.SignUpAsync(signUpDto);
        }
    }    
}
