using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;

namespace Restaurant.Infrastructure.Time
{
    internal static class Extensions
    {
        public static IServiceCollection AddTime(this IServiceCollection services)
        {
            services.AddSingleton<IClock, Clock>();
            return services;
        }
    }
}
