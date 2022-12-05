using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Restaurant.UI.DTO;
using System.Security.Claims;

namespace Restaurant.UI.Security
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<UserDto>("token");

            if (token is null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                { new Claim("id", token.Id.ToString()), new Claim("email", token.Email), new Claim("role", token.Role) }
            )));
        }
    }
}
