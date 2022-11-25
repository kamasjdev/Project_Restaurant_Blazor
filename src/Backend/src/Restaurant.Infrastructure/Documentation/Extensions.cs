using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Restaurant.Infrastructure.Documentation
{
    internal static class Extensions
    {
        public static IServiceCollection AddDocs(this IServiceCollection services)
        {
            services.AddSwaggerGen(swagger =>
            {
                swagger.CustomSchemaIds(s => s.FullName);
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant API",
                    Version = "v1"
                });
            });
            return services;
        }

        public static WebApplication UseDocs(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(swaggerUI =>
            {
                swaggerUI.RoutePrefix = "swagger";
                swaggerUI.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
                swaggerUI.DocumentTitle = "Restaurant API";
            });
            return app;
        }
    }
}
