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
        private const string TOKEN = "token";

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<AuthDto>(TOKEN);
            
            if (token is null)
            {
                return new AuthenticationState(_annonymous);
            }

            return GetAuthenticationState(token.AccessToken);
        }

        // method used for update state
        public async Task UpdateAuthenticationStateAsync(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_annonymous)));
                return;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(GetAuthenticationState(token)));
        }

        private AuthenticationState GetAuthenticationState(string? token)
        {
            var claims = JwtExtensions.ParseClaimsFromJwt(token);

            if (!claims.Any())
            {
                return new AuthenticationState(_annonymous);
            }

            var expireClaim = claims.SingleOrDefault(c => c.Type == "exp");

            if (expireClaim is null)
            {
                return new AuthenticationState(_annonymous);
            }

            var parsed = long.TryParse(expireClaim.Value, out var unixTimeStamp);
            DateTime expireDate = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            expireDate = expireDate.AddSeconds(unixTimeStamp);

            if (!parsed)
            {
                return new AuthenticationState(_annonymous);
            }

            if (expireDate < DateTime.UtcNow)
            {
                return new AuthenticationState(_annonymous);
            }

            // need to specify authenticate type, if not specified user will be annonymous
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: AUTH)));
        }
    }
}
