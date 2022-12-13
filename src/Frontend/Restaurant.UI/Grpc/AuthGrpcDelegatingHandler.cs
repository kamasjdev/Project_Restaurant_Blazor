using Blazored.LocalStorage;
using Restaurant.UI.DTO;
using System.Net.Http.Headers;

namespace Restaurant.UI.Grpc
{
    public class AuthGrpcDelegatingHandler : DelegatingHandler
    {
        private readonly HttpMessageHandler _innerHandler;
        private readonly IServiceProvider _serviceProvider;

        public AuthGrpcDelegatingHandler(HttpMessageHandler innerHandler, IServiceProvider serviceProvider) : base(innerHandler)
        {
            _innerHandler = innerHandler;
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var localStorageService = scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            var token = await localStorageService.GetItemAsync<AuthDto>("token");
            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
