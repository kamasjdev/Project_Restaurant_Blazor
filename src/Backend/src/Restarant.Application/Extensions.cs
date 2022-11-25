using Microsoft.Extensions.DependencyInjection;
using Restarant.Application.Abstractions;
using Restarant.Application.Services;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Restaurant.UnitTests")]
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