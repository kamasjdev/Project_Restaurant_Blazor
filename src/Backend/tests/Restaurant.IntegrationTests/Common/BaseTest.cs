using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Data;

namespace Restaurant.IntegrationTests.Common
{
    [Collection("api")]
    public abstract class BaseTest : IClassFixture<OptionsProvider>, IDisposable
    {
        protected OptionsProvider OptionsProvider { get; }
        private readonly TestAppFactory _app;
        private IServiceScope? _scope;

        public BaseTest(OptionsProvider optionsProvider)
        {
            OptionsProvider = optionsProvider;
            _app = new TestAppFactory(ConfigureServices);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Configure services if needed
        }

        protected T GetRequiredService<T>() where T : notnull
        {
            _scope ??= _app.Services.CreateScope();
            return _scope.ServiceProvider.GetRequiredService<T>();
        }


        public void Dispose()
        {
            _scope ??= _app.Services.CreateScope();
            var connection = _scope.ServiceProvider.GetRequiredService<DbConnection>();
            var logger = _scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation($"Dropping test database {connection.Database}");
                var command = connection.CreateCommand();
                command.CommandText = $"DROP DATABASE IF EXISTS `{connection.Database}`";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "There was an error while drop database");
            }
            finally
            {
                logger.LogInformation("Closing connection. Disposing TestAppFactory");
                _scope.Dispose();
                connection.Close();
                connection.Dispose();
                _app.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
