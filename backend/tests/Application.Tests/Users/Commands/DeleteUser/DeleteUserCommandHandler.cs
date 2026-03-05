using System.Linq.Expressions;

using Application.Abstractions.Data;
using Application.Users.Commands.DeleteUser;

using Domain.Entities;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.DeleteUser;

public class DeleteUserCommandHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private DeleteUserCommandHandler _commandHandler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();

        _unitOfWork.UserRepository.Returns(_userRepository);

        _commandHandler = new DeleteUserCommandHandler(_unitOfWork);
    }

    [Test]
    public async Task Handle_ValidCommand_DeletesUserSuccessfully()
    {
        // Arrange
        var command = new DeleteUserCommand(1);

        _userRepository.ExecuteDeleteAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();

        await _userRepository.Received(1).ExecuteDeleteAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}