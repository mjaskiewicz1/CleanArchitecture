using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Commands.SetPassword;

using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.SetPassword;

public class SetPasswordCommandHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private IPasswordHasher _passwordHasher = null!;

    private SetPasswordCommandHandler _handler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();

        _unitOfWork.UserRepository.Returns(_userRepository);

        _handler = new SetPasswordCommandHandler(_unitOfWork, _passwordHasher);
    }


    [Test]
    public async Task Should_set_password_successfully()
    {
        // Arrange
        var command = new SetPasswordCommand("valid-token", "NewPassword1");
        const string hashedPassword = "hashed-password";

        _passwordHasher.Hash(command.Password).Returns(hashedPassword);


        _userRepository.SetPasswordByTokenAsync(command.Token, hashedPassword, CancellationToken.None).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();

        await _userRepository.Received(1)
            .SetPasswordByTokenAsync(command.Token, hashedPassword, CancellationToken.None);
    }


    [Test]
    public async Task Should_fail_when_token_is_invalid()
    {
        // Arrange
        var command = new SetPasswordCommand("invalid-token", "NewPassword1");
        const string hashedPassword = "hashed-password";

        _passwordHasher.Hash(command.Password).Returns(hashedPassword);


        _userRepository.SetPasswordByTokenAsync(command.Token, hashedPassword, CancellationToken.None)
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();

        await _userRepository.Received(1)
            .SetPasswordByTokenAsync(command.Token, hashedPassword, CancellationToken.None);
    }
}