using Application.Abstractions.Authentication;

using Infrastructure.Authorization;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tests;

using TUnit.Core;


public class MicrosoftDependencyInjectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return ServiceProvider.CreateScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }

    private static ServiceProvider CreateSharedServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services.BuildServiceProvider();
    }

}