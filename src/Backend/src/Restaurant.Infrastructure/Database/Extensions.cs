using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Restarant.Application.Abstractions;
using System.Data.Common;

namespace Restaurant.Infrastructure.Database
{
    internal static class Extensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(configuration.GetRequiredSection("database"));
            services.AddScoped<DbConnection>(sp =>
            {
                var dbOptions = sp.GetRequiredService<IOptionsMonitor<DatabaseOptions>>().CurrentValue;

                if (dbOptions.ConnectionString is null)
                {
                    throw new InvalidOperationException("ConnectionString is empty. Check section appsettings 'database' with key 'connectionString' if is filled and try again");
                }

                var connection = new MySqlConnection(dbOptions.ConnectionString);
                return connection;
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
