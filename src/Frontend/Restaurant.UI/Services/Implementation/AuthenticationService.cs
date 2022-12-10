using Blazored.LocalStorage;
using Restaurant.UI.DTO;
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
            var user = ((UserService)_userService).GetAsync(signInDto.Email);
            var userDto = new UserDto { Id = user.Id, Email = user.Email, CreatedAt = user.CreatedAt, Role = user.Role };
            await localStorageService.SetItemAsync(TOKEN, userDto);
            return userDto;
        }

        public async Task SignoutAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            await localStorageService.RemoveItemAsync(TOKEN);
        }

        public Task SignUpAsync(SignUpDto signUpDto)
        {
            _userService.SignUpAsync(signUpDto);
            return Task.CompletedTask;
        }
    }    
}
