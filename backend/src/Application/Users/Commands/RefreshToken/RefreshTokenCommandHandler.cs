using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Errors;
using Domain.Shared;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = await unitOfWork.RefreshTokenRepository.GetAsync(
            filter: x => x.Token == request.RefreshToken && x.ExpiresAtUtc > DateTime.UtcNow,
            include: x => x.Include(rt => rt.User)
                .ThenInclude(u => u.UserPermissions)
                .ThenInclude(up => up.Permission),
            cancellationToken: cancellationToken);
        if (refreshToken == null)
            return Result<RefreshTokenResponse>.Failure(UserErrors.InvalidToken);
        var accessToken = tokenProvider.CreateAccessToken(refreshToken.User);
        var newRefreshToken = tokenProvider.CreateRefreshToken();
        var refreshTokenHash = passwordHasher.Hash(newRefreshToken);
        await unitOfWork.RefreshTokenRepository.RotateRefreshTokenAsync(refreshToken.Id, refreshTokenHash,
            DateTime.Now.AddMinutes(15), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(new RefreshTokenResponse(accessToken, newRefreshToken, refreshToken.ExpiresAtUtc));
    }
}