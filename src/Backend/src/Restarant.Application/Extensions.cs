using Microsoft.Extensions.DependencyInjection;
using Restarant.Application.Abstractions;
using Restarant.Application.Services;

namespace Restarant.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAdditionService, AdditionService>();
            return services;
        }
    }
}