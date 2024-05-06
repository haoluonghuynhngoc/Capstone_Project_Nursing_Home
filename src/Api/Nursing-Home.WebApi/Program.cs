using Nursing_Home.Application;
using Nursing_Home.Infrastructure;
using Nursing_Home.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add Dependency Injection
builder.Services
    .AddInfrastructure(builder.Configuration);
builder.Services
    .AddApplication(builder.Configuration);
builder.Services
    .AddWebApi(builder.Configuration);

var app = builder.Build();

// Add Dependency Injection
await app.UseWebApplication();

app.Run();
