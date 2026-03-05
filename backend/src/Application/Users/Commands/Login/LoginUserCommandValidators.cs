using Domain.Constants;

using FluentValidation;

namespace Application.Users.Commands.Login;

public class LoginUserCommandValidators : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidators()
    {
        RuleFor(static x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(EntityConstraintsValues.EmailLength);

        RuleFor(static x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}