using Microsoft.Extensions.DependencyInjection;
using Restaurant.Core.Repositories;
using Restaurant.Infrastructure.Repositories.InMemory;

namespace Restaurant.Infrastructure.Repositories
{
    internal static class Extensions
    {
        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IAdditonRepository, InMemoryAdditionRepository>();
            return services;
        }
    }
}
