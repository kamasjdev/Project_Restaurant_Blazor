using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Restaurant.Infrastructure.Repositories;

namespace Restaurant.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSwaggerGen(swagger =>
            {
                swagger.CustomSchemaIds(s => s.FullName);
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant API",
                    Version = "v1"
                });
            });
            services.Configure<AppOptions>(configuration.GetRequiredSection("app"));
            services.AddInMemoryRepositories();
            return services;
        }

        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(swaggerUI =>
            {
                swaggerUI.RoutePrefix = "swagger";
                swaggerUI.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
                swaggerUI.DocumentTitle = "Restaurant API";
            });
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}