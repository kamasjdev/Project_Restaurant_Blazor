using Microsoft.AspNetCore.Builder;
using Restaurant.Infrastructure.Grpc.Services;

namespace Restaurant.Infrastructure.Grpc
{
    internal static class Extensions
    {
        public static WebApplication UseGrpc(this WebApplication app)
        {
            app.UseGrpcWeb();
            app.MapGrpcService<WeatherService>()
                .EnableGrpcWeb();
            app.MapGrpcService<AdditionGrpcService>()
                .EnableGrpcWeb();
            return app;
        }
    }
}
