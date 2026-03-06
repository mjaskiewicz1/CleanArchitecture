using Application.Abstractions.Authentication;
using Application.Abstractions.Data;

using Domain.Errors;
using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.SetPassword;

public sealed class SetPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    : IRequestHandler<SetPasswordCommand, Result>
{
    public async Task<Result> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = passwordHasher.Hash(request.Password);
        if (!await unitOfWork.UserRepository.SetPasswordByTokenAsync(request.Token, hashedPassword, cancellationToken))
            return Result.Failure(UserErrors.InvalidToken);

        return Result.Success();
    }
}