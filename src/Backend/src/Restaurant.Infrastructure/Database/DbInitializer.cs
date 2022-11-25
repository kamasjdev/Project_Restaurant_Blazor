using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Infrastructure.Database
{
    internal sealed class DbInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            await Task.Run(migrationRunner.MigrateUp, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
