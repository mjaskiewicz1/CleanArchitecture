using Application.Tests.Validators;
using Application.Users.Commands.SetPassword;

using FluentValidation.TestHelper;

namespace Application.Tests.Users.Commands.SetPassword;

[MicrosoftDependencyInjectionDataSource]
public sealed class SetPasswordCommandValidatorTests(SetPasswordCommandValidator validator)
    : BaseValidatorTest<SetPasswordCommandValidator>(validator)
{
    #region Empty or Null Field Tests

    [Test]
    [Arguments("", "ValidPass1")]
    public async Task Should_Fail_When_Token_Is_Empty(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Test]
    [Arguments("valid-token", "")]
    public async Task Should_Fail_When_Password_Is_Empty(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region Password Minimum Length Tests

    [Test]
    [Arguments("valid-token", "Short1")]
    [Arguments("valid-token", "Abc123")]
    public async Task Should_Fail_When_Password_Is_Too_Short(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region Password Complexity Tests

    [Test]
    [Arguments("valid-token", "alllowercase1")]
    public async Task Should_Fail_When_Password_Has_No_Uppercase(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    [Arguments("valid-token", "ALLUPPERCASE1")]
    public async Task Should_Fail_When_Password_Has_No_Lowercase(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Test]
    [Arguments("valid-token", "NoDigitsHere")]
    public async Task Should_Fail_When_Password_Has_No_Digit(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region Positive Tests

    [Test]
    [Arguments("valid-token", "ValidPass1")]
    [Arguments("another-token", "SecurePassword123")]
    public async Task Should_Pass_When_All_Fields_Are_Valid(string token, string password)
    {
        // Arrange
        var command = new SetPasswordCommand(token, password);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}