using Blazored.LocalStorage;
using Humanizer;
using Microsoft.AspNetCore.Components.Authorization;
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
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(ILocalStorageService localStorageService, IUserService userService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _localStorageService = localStorageService;
            _userService = userService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<AuthDto> SignInAsync(SignInDto signInDto)
        {
            var auth = await _userService.SignInAsync(signInDto);
            await _localStorageService.SetItemAsync(TOKEN, auth);
            await ((CustomAuthenticationStateProvider)_authenticationStateProvider).UpdateAuthenticationStateAsync(auth.AccessToken);
            return auth;
        }

        public async Task SignoutAsync()
        {
            await _localStorageService.RemoveItemAsync(TOKEN);
            await ((CustomAuthenticationStateProvider)_authenticationStateProvider).UpdateAuthenticationStateAsync(null);
        }

        public async Task SignUpAsync(SignUpDto signUpDto)
        {
            await _userService.SignUpAsync(signUpDto);
        }
    }    
}
