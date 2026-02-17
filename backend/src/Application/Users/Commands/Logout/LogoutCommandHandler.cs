using Application.Abstractions.Data;

using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.Logout;

public sealed class LogoutCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.RefreshTokenRepository.ExecuteDeleteAsync(filter: x => x.UserId == request.UserId,
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}