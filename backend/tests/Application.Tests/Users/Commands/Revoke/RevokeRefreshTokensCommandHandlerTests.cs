using System.Linq.Expressions;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Commands.Revoke;

using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.Revoke;

public class RevokeRefreshTokensCommandHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IRefreshTokenRepository _refreshTokenRepository = null!;
    private IUserContext _userContext = null!;
    private RevokeRefreshTokensCommandHandler _handler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        _userContext = Substitute.For<IUserContext>();

        _unitOfWork.RefreshTokenRepository.Returns(_refreshTokenRepository);

        _handler = new RevokeRefreshTokensCommandHandler(_unitOfWork, _userContext);
    }

    [Test]
    public async Task Should_delete_refresh_tokens_for_current_user_and_return_success()
    {
        // Arrange
        const ulong userId = 123ul;
        _userContext.UserId.Returns(userId);

        var command = new RevokeRefreshTokensCommand();

        Expression<Func<Domain.Entities.RefreshToken, bool>>? capturedFilter = null;

        _refreshTokenRepository
            .ExecuteDeleteAsync(
                Arg.Do<Expression<Func<Domain.Entities.RefreshToken, bool>>>(f => capturedFilter = f),
                CancellationToken.None)
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();

        await _refreshTokenRepository.Received(1)
            .ExecuteDeleteAsync(
                Arg.Any<Expression<Func<Domain.Entities.RefreshToken, bool>>>(),
                CancellationToken.None);
        
        // Turn the captured expression tree into a real function we can run.
        // Expression trees describe code but can't run by themselves.
        // Compile() makes a Func<RefreshToken, bool> we can call to test tokens.
        var compiled = capturedFilter!.Compile();

        var validToken = new Domain.Entities.RefreshToken
        {
            Token = "hash", ExpiresAtUtc = DateTime.UtcNow, UserId = userId
        };

        var invalidToken = new Domain.Entities.RefreshToken
        {
            Token = "hash", ExpiresAtUtc = DateTime.UtcNow, UserId = 999ul
        };

        await Assert.That(compiled(validToken)).IsTrue();
        await Assert.That(compiled(invalidToken)).IsFalse();
    }
}