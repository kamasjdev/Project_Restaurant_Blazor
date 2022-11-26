using Restaurant.Application;
using Restaurant.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();  

// Configure the HTTP request pipeline.
app.UseInfrastructure();

app.Run();

public partial class Program { }
