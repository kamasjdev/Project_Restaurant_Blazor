using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Restaurant.IntegrationTests.Common
{
    internal sealed class TestAppFactory : WebApplicationFactory<Program>
    {
        public HttpClient Client { get; }

        public TestAppFactory(Action<IServiceCollection> services = null)
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
    }
}