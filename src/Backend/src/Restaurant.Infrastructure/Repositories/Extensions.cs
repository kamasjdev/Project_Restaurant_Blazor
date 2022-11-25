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
            services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            services.AddSingleton<IProductSaleRepository, InMemoryProductSaleRepository>();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            return services;
        }
    }
}
