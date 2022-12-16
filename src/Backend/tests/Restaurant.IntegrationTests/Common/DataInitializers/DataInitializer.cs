using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Application.Abstractions;
using Restaurant.Core.Repositories;

namespace Restaurant.IntegrationTests.Common.DataInitializers
{
    internal sealed class DataInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DataInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var user = TestData.GetAdminUser();
            var passwordManager = _serviceProvider.GetRequiredService<IPasswordManager>();
            var securedPassword = passwordManager.Secure(user.Password);
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            user.ChangePassword(securedPassword);
            await userRepository.AddAsync(user);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
