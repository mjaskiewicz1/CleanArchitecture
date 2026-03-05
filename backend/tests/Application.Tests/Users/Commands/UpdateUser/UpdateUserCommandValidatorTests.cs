using Application.Tests.Validators;
using Application.Users.Commands.UpdateUser;

using Domain.Constants;

using FluentValidation.TestHelper;

namespace Application.Tests.Users.Commands.UpdateUser;

[MicrosoftDependencyInjectionDataSource]
public sealed class UpdateUserCommandValidatorTests(UpdateUserCommandValidators validator)
    : BaseValidatorTest<UpdateUserCommandValidators>(validator)
{
    #region Empty or Null Field Tests

    [Test]
    [Arguments("", "Doe", "john.doe@example.com", "FirstName")]
    [Arguments("John", "", "john.doe@example.com", "LastName")]
    [Arguments("John", "Doe", "", "Email")]
    public async Task Should_Fail_When_Field_Is_Empty(string firstName, string lastName, string email,
        string expectedProperty)
    {
        // Arrange
        var command = new UpdateUserCommand(firstName, lastName, email, []);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(expectedProperty);
    }

    #endregion

    #region Invalid Email Tests

    [Test]
    [Arguments("John", "Doe", "invalid-email")]
    [Arguments("Jane", "Doe", "also.invalid@")]
    public async Task Should_Fail_When_Email_Is_Invalid(string firstName, string lastName, string email)
    {
        // Arrange
        var command = new UpdateUserCommand(firstName, lastName, email, []);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Maximum Length Tests

    [Test]
    public async Task Should_Fail_When_FirstName_Exceeds_MaxLength()
    {
        // Arrange
        var tooLong = new string('A', EntityConstraintsValues.NameLength + 1);

        var command = new UpdateUserCommand(tooLong, "Doe", "john@example.com", []);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Test]
    public async Task Should_Fail_When_LastName_Exceeds_MaxLength()
    {
        // Arrange
        var tooLong = new string('B', EntityConstraintsValues.NameLength + 1);

        var command = new UpdateUserCommand("John", tooLong, "john@example.com", []);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Test]
    public async Task Should_Fail_When_Email_Exceeds_MaxLength()
    {
        // Arrange
        var tooLongEmail = new string('a', EntityConstraintsValues.EmailLength + 1) + "@a.com";

        var command = new UpdateUserCommand("John", "Doe", tooLongEmail, []);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Positive Tests

    [Test]
    [Arguments("John", "Doe", "john.doe@example.com")]
    [Arguments("Alice", "Smith", "alice.smith@example.com")]
    public async Task Should_Pass_When_All_Fields_Are_Valid(string firstName, string lastName, string email)
    {
        // Arrange
        var command = new UpdateUserCommand(firstName, lastName, email, []);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}