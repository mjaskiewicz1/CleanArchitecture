using Application.Tests.Validators;
using Application.Users.Commands.Login;

namespace Application.Tests.Users.Commands.Login;

[MicrosoftDependencyInjectionDataSource]
public sealed class LoginUserCommandValidatorTests(LoginUserCommandValidators validator)
    : BaseValidatorTest<LoginUserCommandValidators>(validator)
{
    [Test]
    [Arguments("", "password123", "Email")]
    [Arguments("test@example.com", "", "Password")]
    public async Task Should_Fail_When_Field_Is_Empty(string email, string password, string expectedField)
    {
        // Arrange
        var command = new LoginUserCommand(email, password);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        await Assert.That(result.IsValid).IsFalse();
        await Assert.That(result.Errors).Contains(x => x.PropertyName == expectedField);
    }

    [Test]
    [Arguments("invalid-email", "password123")]
    public async Task Should_Fail_When_Field_Does_Not_Match_Validation_Rules(string email, string password)
    {
        // Arrange
        var command = new LoginUserCommand(email, password);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    [Arguments("test@example.com", "password123")]
    public async Task Should_Pass_When_Field_Matches_Validation_Rules(string email, string password)
    {
        // Arrange
        var command = new LoginUserCommand(email, password);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        await Assert.That(result.IsValid).IsTrue();
    }
}