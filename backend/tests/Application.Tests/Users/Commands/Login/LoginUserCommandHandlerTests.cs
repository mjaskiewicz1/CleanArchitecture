using System.Linq.Expressions;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Commands.Login;

using Domain.Entities;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.Login;

public class LoginUserCommandHandlerTests
{

    private IUserRepository _userRepository = null!;
    private IRefreshTokenRepository _refreshTokenRepository = null!;
    private IPasswordHasher _passwordHasher = null!;
    private ITokenProvider _tokenProvider = null!;
    private IUnitOfWork _unitOfWork = null!;
    private LoginUserCommandHandler _handler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _tokenProvider = Substitute.For<ITokenProvider>();

        _unitOfWork.UserRepository.Returns(_userRepository);
        _unitOfWork.RefreshTokenRepository.Returns(_refreshTokenRepository);

        _handler = new LoginUserCommandHandler(_unitOfWork, _passwordHasher, _tokenProvider);
    }

    [Test]
    public async Task Should_login_user_and_return_tokens()
    {
        var command = new LoginUserCommand("test@test.com", "Password123");

        var user = new User
        {
            Email = command.Email,
            PasswordHash = "stored-password-hash",
            FirstName = "Name",
            LastName = "Surname",
        };

        _userRepository.GetAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
                include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
                cancellationToken: CancellationToken.None)
            .Returns(user);

        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);

        _tokenProvider.CreateAccessToken(user).Returns("access-token");

        _tokenProvider.CreateRefreshToken().Returns("refresh-token");

        _passwordHasher.Hash("refresh-token").Returns("hashed-refresh-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();

        await Assert.That(result.Value.AccessToken).IsEqualTo("access-token");
        await Assert.That(result.Value.RefreshToken).IsEqualTo("hashed-refresh-token");

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Test]
    public async Task Should_not_login_user_and_return_error_when_user_does_not_exist()
    {
        var command = new LoginUserCommand("test@test.com", "Password123");

        _userRepository.GetAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
                include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
                cancellationToken: CancellationToken.None).Returns((User)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
    }
}