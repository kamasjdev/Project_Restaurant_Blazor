using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Restaurant.Application.Abstractions;
using Restaurant.Infrastructure.Repositories;
using Restaurant.Migrations;
using System.Data.Common;

namespace Restaurant.Infrastructure.Database
{
    internal static class Extensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection("database");
            var connectionString = section.GetRequiredSection("connectionString").Value;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionString is empty. Check section appsettings 'database' with key 'connectionString' if is filled and try again");
            }

            CreateDatabaseIfNotExists(connectionString);
            services.Configure<DatabaseOptions>(section);
            services.AddScoped<DbConnection>(sp =>
            {
                var dbOptions = sp.GetRequiredService<IOptionsMonitor<DatabaseOptions>>().CurrentValue;
                var connection = new MySqlConnection(dbOptions.ConnectionString);
                connection.Open();
                return connection;
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMigrations(connectionString);
            services.AddDatabaseInitializer();
            services.AddInMemoryRepositories();
            services.AddRepositories();
            
            return services;
        }

        public static IServiceCollection AddMigrations(this IServiceCollection services, string connectionString)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(cr =>
                            cr.AddMySql5()
                               .WithGlobalConnectionString(connectionString)
                               .ScanIn(typeof(InitCreateAdditionTable).Assembly).For.Migrations())
                .AddLogging(l => l.AddFluentMigratorConsole());
            return services;
        }

        public static IServiceCollection AddDatabaseInitializer(this IServiceCollection services)
        {
            services.AddHostedService<DbInitializer>();
            return services;
        }

        private static void CreateDatabaseIfNotExists(string connectionString)
        {
            var connectionStringSplited = connectionString.Split(";");
            var connectionStringWithoutDb = connectionStringSplited.Where(str => !str.Contains("Database="))
                                .Aggregate((current, next) => current + ";" + next);
            var database = connectionStringSplited.SingleOrDefault(str => str.Contains("Database="))?.Split("Database=")[1];

            if (string.IsNullOrEmpty(database))
            {
                throw new InvalidOperationException("Invalid ConnectionString. There is no value for 'Database=' check it and try again");
            }

            using var conn = new MySqlConnection(connectionStringWithoutDb);
            using var cmd = conn.CreateCommand();
            conn.Open();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{database}`";
            cmd.ExecuteNonQuery();
        }
    }
}
