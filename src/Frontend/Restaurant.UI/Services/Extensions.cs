using Restaurant.UI.Services.Abstractions;
using Restaurant.UI.Services.Implementation;

namespace Restaurant.UI.Services
{
	internal static class Extensions
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddSingleton<IAdditionService, AdditionService>();
			services.AddSingleton<IProductService, ProductService>();
			services.AddSingleton<IAuthenticationService, AuthenticationService>();
			services.AddSingleton<IProductSaleService, ProductSaleService>();
			return services;
		}
	}
}
