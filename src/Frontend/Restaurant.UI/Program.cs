using Restaurant.Shared;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Restaurant.UI;
using Restaurant.Shared.AdditionProto;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton(services =>
{
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    //var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
    var baseUri = "http://localhost:5000"; // TODO add from appsettings
    var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
    return new WeatherForecasts.WeatherForecastsClient(channel);
});
builder.Services.AddSingleton(services =>
{
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    //var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
    var baseUri = "http://localhost:5000"; // TODO add from appsettings
    var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
    return new Additions.AdditionsClient(channel);
});

await builder.Build().RunAsync();
