using NetArchTest.Rules;

namespace Architecture.Tests.Layers;

public sealed class LayerTests : BaseTest
{
    [Test]
    public async Task Domain_Should_NotHaveDependencyOnApplication()
    {
        // Arrange & Act
        var res = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        // Assert
        await Assert.That(res.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        // Assert
        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();

        // Assert
        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        // Arrange & Act
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();

        // Assert
        await Assert.That(result.IsSuccessful).IsTrue();
    }
}