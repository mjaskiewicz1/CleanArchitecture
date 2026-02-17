using System.Linq.Expressions;

using Application.Abstractions.Data;
using Application.Users.Commands.Logout;

using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.Logout;

public class LogoutCommandHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IRefreshTokenRepository _refreshTokenRepository = null!;
    private LogoutCommandHandler _handler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

        _unitOfWork.RefreshTokenRepository.Returns(_refreshTokenRepository);

        _handler = new LogoutCommandHandler(_unitOfWork);
    }

    [Test]
    public async Task Should_delete_refresh_tokens_for_user_and_return_success()
    {
        var userId = 123ul;
        var command = new LogoutCommand(userId);

        _refreshTokenRepository.ExecuteDeleteAsync(Arg.Any<Expression<Func<Domain.Entities.RefreshToken, bool>>>(),
                CancellationToken.None)
            .Returns(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();

        await _refreshTokenRepository.Received(1)
            .ExecuteDeleteAsync(Arg.Any<Expression<Func<Domain.Entities.RefreshToken, bool>>>(),
                CancellationToken.None);
    }
}