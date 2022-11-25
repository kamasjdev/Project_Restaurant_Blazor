using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Repositories;

namespace Restaurant.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) 
        {
            services.AddInMemoryRepositories();
            return services;
        }
    }
}