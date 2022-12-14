using Restaurant.UI.Services.Abstractions;
using Restaurant.UI.Services.Implementation;

namespace Restaurant.UI.Services
{
	internal static class Extensions
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddScoped<IAdditionService, AdditionService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IProductSaleService, ProductSaleService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IUserService, UserService>();
			return services;
		}
	}
}
