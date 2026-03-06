using FluentValidation;

namespace Application.Users.Commands.SetPassword;

public sealed class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
{
    public SetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            // password must have:
            // - one lowercase letter (a-z)
            // - one uppercase letter (A-Z)
            // - one number (0-9)
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");
    }
}