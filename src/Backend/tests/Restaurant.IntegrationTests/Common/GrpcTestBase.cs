using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Abstractions;
using Restaurant.IntegrationTests.Common.DataInitializers;
using System.Net.Http.Headers;

namespace Restaurant.IntegrationTests.Common
{
    public abstract class GrpcTestBase : BaseTest
    {
        private GrpcChannel? _channel;

        protected GrpcChannel Channel => _channel ??= CreateChannel();

        protected GrpcChannel CreateChannel()
        {
            return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpHandler = new GrpcDelegatingHandler(Fixture.Server.CreateHandler())
            });
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<DataInitializer>();
            base.ConfigureServices(services);
        }

        public GrpcTestBase(OptionsProvider optionsProvider) : base(optionsProvider)
        {
            SetBearerToken();
        }

        private static string _token = "";

        private void SetBearerToken()
        {
            var authManager = GetRequiredService<IJwtManager>();
            var user = TestData.GetAdminUser();
            _token = authManager.CreateToken(user.Id, user.Role, user.Email.Value);
        }

        public override void Dispose()
        {
            _channel = null;
            base.Dispose();
        }

        private class GrpcDelegatingHandler : DelegatingHandler
        {
            public GrpcDelegatingHandler(HttpMessageHandler handler)
            {
                InnerHandler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
