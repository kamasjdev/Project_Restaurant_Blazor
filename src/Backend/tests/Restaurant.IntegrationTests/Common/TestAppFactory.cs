using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Restaurant.IntegrationTests.Common
{
    internal sealed class TestAppFactory : WebApplicationFactory<Program>
    {
        public HttpClient Client { get; }

        public TestAppFactory(Action<IServiceCollection>? services = null)
        {
            Client = WithWebHostBuilder(builder =>
            {
                if (services is not null)
                {
                    builder.ConfigureServices(services);
                }

                builder.UseEnvironment("test");
            }).CreateClient();
        }

        public override async ValueTask DisposeAsync()
        {
            var scope = Services.CreateScope();
            var connection = scope.ServiceProvider.GetRequiredService<DbConnection>();
            var logger = Services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation($"Dropping test database {connection.Database}");
                var command = connection.CreateCommand();
                command.CommandText = $"DROP DATABASE {connection.Database}";
                command.CommandType = CommandType.Text;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "There was an error while drop database");
            }
            finally
            {
                logger.LogInformation("Closing connection. Disposing TestAppFactory");
                scope.Dispose();
                await connection.CloseAsync();
                await connection.DisposeAsync();
                await base.DisposeAsync();
            }
        }
    }
}