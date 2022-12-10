using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Restaurant.UI.DTO;
using System.Security.Claims;

namespace Restaurant.UI.Security
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _annonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private const string AUTH = "JwTAuth";

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<UserDto>("token");

            if (token is null)
            {
                return new AuthenticationState(_annonymous);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                { new Claim("id", token.Id.ToString()), new Claim("email", token.Email), new Claim(ClaimTypes.Role, token.Role) }
            , authenticationType: AUTH))); // need to specify authenticate type, if not specified user will be annonymous
        }

        // method used for update state
        public async Task UpdateAuthenticationStateAsync(UserDto? userDto)
        {
            if (userDto is null)
            {
                await _localStorage.RemoveItemAsync("token");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_annonymous)));
                return;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                { new Claim("id", userDto.Id.ToString()), new Claim("email", userDto.Email), new Claim("role", userDto.Role) }
            , authenticationType: AUTH)))));
        }
    }
}
