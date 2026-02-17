namespace Application.Tests;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

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
        var applicationAssembly = typeof(DependencyInjection).Assembly;
        services.AddValidatorsFromAssembly(applicationAssembly);
        return services.BuildServiceProvider();
    }
}