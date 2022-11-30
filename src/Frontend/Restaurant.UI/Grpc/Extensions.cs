using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Restaurant.Shared.AdditionProto;
using Restaurant.Shared;
using Restaurant.Shared.OrderProto;
using Restaurant.Shared.ProductProto;
using Restaurant.Shared.ProductSaleProto;
using Restaurant.Shared.UserProto;

namespace Restaurant.UI.Grpc
{
	internal static class Extensions
	{
		public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
		{
			var backendUrl = configuration["externalClients:backend"];

			if (backendUrl is null)
			{
				throw new InvalidOperationException("Invalid backend url");
			}

			services.AddSingleton(services =>
			{
				var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
				var baseUri = backendUrl;
				var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
				return new WeatherForecasts.WeatherForecastsClient(channel);
			});
			services.AddSingleton(services =>
			{
				var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
				var baseUri = backendUrl;
				var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
				return new Additions.AdditionsClient(channel);
			});
			services.AddSingleton(services =>
			{
				var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
				var baseUri = backendUrl;
				var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
				return new Orders.OrdersClient(channel);
			});
			services.AddSingleton(services =>
			{
				var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
				var baseUri = backendUrl;
				var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
				return new Products.ProductsClient(channel);
			});
			services.AddSingleton(services =>
			{
				var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
				var baseUri = backendUrl;
				var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
				return new ProductSales.ProductSalesClient(channel);
			});
			services.AddSingleton(services =>
			{
				var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
				var baseUri = backendUrl;
				var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
				return new Users.UsersClient(channel);
			});
			return services;
		}
	}
}
