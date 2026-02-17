using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.Login;

public class LoginUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetAsync(filter: x => x.Email == request.Email,
            include: x =>
                x.Include(u => u.UserPermissions)
                    .ThenInclude(up => up.Permission),
            cancellationToken: cancellationToken);
        if (user?.PasswordHash is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure(Error.Unauthorized("Invalid email or password."));

        var accessToken = tokenProvider.CreateAccessToken(user);

        var refreshToken = tokenProvider.CreateRefreshToken();
        var refreshTokenHash = passwordHasher.Hash(refreshToken);

        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = refreshTokenHash,
            ExpiresAtUtc = expiresAt,
            UserId = user.Id,
        };
        user.LastLogin = DateTimeOffset.UtcNow;
        await unitOfWork.RefreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse(accessToken, refreshTokenEntity.Token, expiresAt));
    }
}