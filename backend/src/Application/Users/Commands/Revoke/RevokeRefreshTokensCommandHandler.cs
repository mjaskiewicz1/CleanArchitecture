using Application.Abstractions.Authentication;
using Application.Abstractions.Data;

using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.Revoke;

public sealed class RevokeRefreshTokensCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<RevokeRefreshTokensCommand, Result>
{
    public async Task<Result> Handle(RevokeRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.RefreshTokenRepository.ExecuteDeleteAsync(filter: x => x.UserId == userContext.UserId,
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}