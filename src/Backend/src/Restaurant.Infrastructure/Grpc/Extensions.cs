using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Grpc.Interceptors;
using Restaurant.Infrastructure.Grpc.Services;

namespace Restaurant.Infrastructure.Grpc
{
    internal static class Extensions
    {
        public static IServiceCollection AddGrpcCommunication(this IServiceCollection services) 
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<GrpcExceptionInterceptor>();
            });
            return services;
        }

        public static WebApplication UseGrpc(this WebApplication app)
        {
            app.UseGrpcWeb();
            app.MapGrpcService<AdditionGrpcService>()
                .EnableGrpcWeb();
            app.MapGrpcService<OrderGrpcService>()
                .EnableGrpcWeb();
            app.MapGrpcService<ProductGrpcService>()
                .EnableGrpcWeb();
            app.MapGrpcService<ProductSaleGrpcService>()
                .EnableGrpcWeb();
            app.MapGrpcService<UserGrpcService>()
                .EnableGrpcWeb();
            return app;
        }
    }
}
