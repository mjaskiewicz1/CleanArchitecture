using Application.Users.Commands.Login;

using Domain.Constants;

using FluentValidation;

using JetBrains.Annotations;

namespace Application.Validators;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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