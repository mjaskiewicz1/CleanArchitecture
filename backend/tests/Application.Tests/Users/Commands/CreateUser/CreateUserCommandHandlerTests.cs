using System.Collections.Immutable;
using System.Linq.Expressions;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Email;
using Application.Users.Commands.CreateUser;

using Domain.Entities;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private IPermissionRepository _permissionRepository = null!;
    private ITokenProvider _tokenProvider = null!;
    private IEmailSender _emailSender = null!;
    private IEmailContentFactory _emailFactory = null!;

    private CreateUserCommandHandler _handler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _permissionRepository = Substitute.For<IPermissionRepository>();
        _tokenProvider = Substitute.For<ITokenProvider>();
        _emailSender = Substitute.For<IEmailSender>();
        _emailFactory = Substitute.For<IEmailContentFactory>();

        _unitOfWork.UserRepository.Returns(_userRepository);
        _unitOfWork.PermissionRepository.Returns(_permissionRepository);

        _handler = new CreateUserCommandHandler(_unitOfWork, _tokenProvider, _emailSender, _emailFactory);
    }

    [Test]
    public async Task Should_create_user_successfully()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "test@test.com", ImmutableHashSet.Create<ulong>(1, 2));

        _userRepository.ExistsAsync(Arg.Any<Expression<Func<User, bool>>>(), CancellationToken.None).Returns(false);

        _permissionRepository.AllExistAsync(command.PermissionIds, CancellationToken.None).Returns(true);

        _tokenProvider.CreatePasswordResetToken().Returns("reset-token");

        _emailFactory.CreateWelcomeSetPasswordEmail(Arg.Any<User>()).Returns(("Subject", "Body"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();

        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(command.Email, "Subject", "Body", CancellationToken.None);
    }

    [Test]
    public async Task Should_fail_when_email_already_exists()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "test@test.com", [1UL]);

        _userRepository.ExistsAsync(Arg.Any<Expression<Func<User, bool>>>(), CancellationToken.None).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        await _emailSender.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Should_fail_when_permissions_do_not_exist()
    {
        // Arrange
        var command = new CreateUserCommand("John", "Doe", "test@test.com", [1UL]);

        _userRepository.ExistsAsync(Arg.Any<Expression<Func<User, bool>>>(), CancellationToken.None).Returns(false);

        _permissionRepository.AllExistAsync(command.PermissionIds, CancellationToken.None).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        await _emailSender.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}