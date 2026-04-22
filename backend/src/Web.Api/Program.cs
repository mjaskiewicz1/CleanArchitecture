using Application;

using Infrastructure.Extensions;

using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddPresentation(builder.Environment.IsDevelopment());


builder.ConfigureInfrastructure();


var app = builder.Build();
app.MapPresentation(builder.Environment.IsDevelopment());
await app.Services.InitializeInfrastructureAsync(app.Environment.IsDevelopment());
await app.RunAsync();