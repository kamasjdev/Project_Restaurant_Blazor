using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Restaurant.UI;
using Restaurant.UI.Grpc;
using Restaurant.UI.Security;
using Restaurant.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddOptions();
builder.Services.AddAuth();

await builder.Build().RunAsync();
