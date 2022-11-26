using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.Application.Services;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Restaurant.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Restaurant.Application
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