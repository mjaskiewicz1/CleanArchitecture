using Domain.Constants;

using FluentValidation;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandValidators : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidators()
    {
        RuleFor(static x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(EntityConstraintsValues.NameLength);

        RuleFor(static x => x.LastName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(EntityConstraintsValues.NameLength);

        RuleFor(static x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(EntityConstraintsValues.EmailLength);
    }
}