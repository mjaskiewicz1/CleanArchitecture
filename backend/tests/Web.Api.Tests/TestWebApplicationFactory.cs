using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Users.Commands.Login;
using Application.Users.Dtos;

using Infrastructure.Database;
using Infrastructure.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.MsSql;

using TUnit.Core.Interfaces;

using Web.Api.Tests.Extensions;

namespace Web.Api.Tests;

internal sealed class TestWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncInitializer
{
    private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's ApplicationDbContext registration.
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            // Register the test database
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.InitializeInfrastructureAsync(true);
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string login, string password)
    {
        var client = CreateClient();

        var url = EndpointPathMapping.Users.Base.ToRelativeUri(EndpointPathMapping.Users.Login);

        var command = new LoginUserCommand(login, password);
        var response = await client.PostAsJsonAsync(url, command);

        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse!.AccessToken);

        return client;
    }
}