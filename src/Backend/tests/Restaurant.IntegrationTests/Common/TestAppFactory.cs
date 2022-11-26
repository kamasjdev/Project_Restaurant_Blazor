using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

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

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("test");
        }
    }
}