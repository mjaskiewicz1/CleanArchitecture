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

    public const string PasswordHash =
        "784B97F150C499520BEE93F7193B8FA37BC5917ACFC5F55D1B1516B46B98023F-A65B186D61C9D4C17341A33F44317755";

    public TestDataSeeder Seeder => new(Services);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.InitializeInfrastructureAsync(true);
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string login = "admin@admin.com", string password = "test")
    {
        var client = CreateClient();
        var url = EndpointPathMapping.Users.Base.ToRelativeUri(EndpointPathMapping.Users.Login);
        var command = new LoginUserCommand(login, password);
        var response = await client.PostAsJsonAsync(url, command);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse!.AccessToken);
        return client;
    }

    public ApplicationDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}