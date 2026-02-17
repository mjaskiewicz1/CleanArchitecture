using Application.Abstractions.Authentication;

namespace Infrastructure.Tests.Authorization;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static",
    Justification = "Disabled for this class")]
[MicrosoftDependencyInjectionDataSource]
public class PasswordHasherTests(IPasswordHasher hasher)
{

    [Test]
    [Arguments("test")]
    public async Task VerifyPassword_Returns_True_Password_Is_Correct(string password)
    {
        // Arrange
        var passwordHash = hasher.Hash(password);
        // Act
        var result = hasher.Verify(password, passwordHash);
        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("test", "wrong")]
    public async Task VerifyPassword_Returns_False_Password_Is_Incorrect(string password, string wrongPassword)
    {
        // Arrange
        var passwordHash = hasher.Hash(password);
        // Act
        var result = hasher.Verify(wrongPassword, passwordHash);
        // Assert
        await Assert.That(result).IsFalse();
    }
    [Test]
    [Arguments("test")]
    public async Task HashPassword_Returns_Hash(string password)
    {
        // Act
        var result = hasher.Hash(password);
        // Assert
        await Assert.That(result).IsNotEmpty();
    }
}