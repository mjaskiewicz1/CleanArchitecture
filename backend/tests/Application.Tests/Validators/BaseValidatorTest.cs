using FluentValidation;

namespace Application.Tests.Validators;

public abstract class BaseValidatorTest<TValidator>(TValidator validator)
    where TValidator : IValidator
{
    protected readonly TValidator _validator = validator;
}