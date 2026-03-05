using Domain.Constants;

using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidators : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidators()
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