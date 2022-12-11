using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Database;
using Restaurant.Infrastructure.Documentation;
using Restaurant.Infrastructure.Exceptions;
using Restaurant.Infrastructure.Grpc;
using Restaurant.Infrastructure.Security;
using Restaurant.Infrastructure.Time;

namespace Restaurant.Infrastructure
{
    public static class Extensions
    {
        private const string CorsPolicy = "CorsPolicy";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDocs();
            services.AddControllers();
            services.AddErrorHandling();
            services.AddDatabase(configuration);
            services.AddTime();
            services.AddAuth(configuration);
            services.AddGrpcCommunication();
            services.Configure<AppOptions>(configuration.GetRequiredSection("app"));
            services.AddCors(cors => cors.AddPolicy(CorsPolicy, options =>
            {
                options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message");
            }));
            return services;
        }

        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseCors(CorsPolicy);
            app.UseDocs();
            app.UseErrorHandling();
            app.UseAuthorization();
            app.UseGrpc();
            app.MapControllers();
            return app;
        }

        public static T GetOptions<T>(this IConfiguration configuration, string sectionName)
           where T : class, new()
        {
            var options = new T();
            var section = configuration.GetRequiredSection(sectionName);
            section.Bind(options);
            return options;
        }
    }
}