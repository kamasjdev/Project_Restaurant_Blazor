using Microsoft.Extensions.DependencyInjection;
using MySqlX.XDevAPI;

namespace Restaurant.IntegrationTests.Common
{
    [Collection("api")]
    public abstract class BaseTest : IClassFixture<OptionsProvider>
    {
        protected OptionsProvider OptionsProvider { get; }

        public BaseTest(OptionsProvider optionsProvider)
        {
            OptionsProvider = optionsProvider;
            var app = new TestAppFactory(ConfigureServices);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Configure services if needed
        }
    }
}
