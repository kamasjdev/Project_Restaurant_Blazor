using Microsoft.Extensions.DependencyInjection;

namespace Restaurant.IntegrationTests.Common
{
    [Collection("api")]
    public abstract class BaseTest : IClassFixture<OptionsProvider>, IDisposable
    {
        protected OptionsProvider OptionsProvider { get; }
        private readonly TestAppFactory _app;
        private AsyncServiceScope? _scope;

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
            _scope ??= _app.Services.CreateAsyncScope();
            return _scope.Value.ServiceProvider.GetRequiredService<T>();
        }


        public void Dispose()
        {
            if (_scope.HasValue)
            {
                _scope.Value.Dispose();
            }

            _app.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
