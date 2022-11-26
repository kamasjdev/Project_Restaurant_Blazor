using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Database;
using Restaurant.Infrastructure.Documentation;
using Restaurant.Infrastructure.Exceptions;
using Restaurant.Infrastructure.Time;

namespace Restaurant.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDocs();
            services.AddControllers();
            services.AddErrorHandling();
            services.AddDatabase(configuration);
            services.AddTime();
            services.Configure<AppOptions>(configuration.GetRequiredSection("app"));
            return services;
        }

        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseDocs();
            app.UseErrorHandling();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}