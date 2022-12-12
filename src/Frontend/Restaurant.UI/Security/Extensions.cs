using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Restaurant.UI.Security
{
    public static class Extensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            return services;
        }
    }
}
