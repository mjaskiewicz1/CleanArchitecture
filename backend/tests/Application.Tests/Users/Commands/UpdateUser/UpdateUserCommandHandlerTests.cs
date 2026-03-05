using System.Linq.Expressions;

using Application.Abstractions.Data;
using Application.Users.Commands.UpdateUser;

using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private IPermissionRepository _permissionRepository = null!;
    private UpdateUserCommandHandler _handler = null!;

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _permissionRepository = Substitute.For<IPermissionRepository>();

        _unitOfWork.UserRepository.Returns(_userRepository);
        _unitOfWork.PermissionRepository.Returns(_permissionRepository);

        _handler = new UpdateUserCommandHandler(_unitOfWork);
    }

    [Test]
    public async Task Handle_ValidCommand_UpdatesUserSuccessfully()
    {
        // Arrange
        var command = new UpdateUserCommand("John", "Doe", "john@test.com", [10UL, 20UL]);
        var existingUser = new User { Id = 1UL, FirstName = "Old", LastName = "Name", Email = "old@test.com" };
        var permissions = new List<Permission>
        {
            new() { Id = 10UL, Name = PermissionName.UserRead },
            new() { Id = 20UL, Name = PermissionName.UserCreate }
        };

        _userRepository.GetAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
            asNoTracking: false,
            cancellationToken: Arg.Any<CancellationToken>()).Returns(existingUser);
        _userRepository.ExistsAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _permissionRepository.GetAllAsync(filter: Arg.Any<Expression<Func<Permission, bool>>>(),
            include: Arg.Any<Func<IQueryable<Permission>, IQueryable<Permission>>>(),
            selector: Arg.Any<Expression<Func<Permission, Permission>>>(), asNoTracking: false,
            cancellationToken: Arg.Any<CancellationToken>()).Returns(permissions);
        _userRepository.UpdateUserPermissionsAsync(user: existingUser, permissions: permissions,
            cancellationToken: Arg.Any<CancellationToken>()).Returns(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await _userRepository.Received(1).UpdateUserPermissionsAsync(user: existingUser, permissions: permissions,
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateUserCommand("John", "Doe", "john@test.com", [10UL]);

        _userRepository.GetAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
            asNoTracking: Arg.Any<bool>(),
            cancellationToken: Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await _userRepository.DidNotReceive().ExistsAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
        await _permissionRepository.DidNotReceive().GetAllAsync(filter: Arg.Any<Expression<Func<Permission, bool>>>(),
            include: Arg.Any<Func<IQueryable<Permission>, IQueryable<Permission>>>(),
            selector: Arg.Any<Expression<Func<Permission, Permission>>>(), asNoTracking: Arg.Any<bool>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_EmailAlreadyTaken_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateUserCommand("John", "Doe", "taken@test.com", [10UL]);

        _userRepository
            .GetAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
                include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
                asNoTracking: false,
                cancellationToken: Arg.Any<CancellationToken>()).Returns(new User
                {
                    Id = 1UL,
                    Email = "old@test.com",
                    FirstName = "Old",
                    LastName = "User"
                });
        _userRepository.ExistsAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await _permissionRepository.DidNotReceive().GetAllAsync(filter: Arg.Any<Expression<Func<Permission, bool>>>(),
            include: Arg.Any<Func<IQueryable<Permission>, IQueryable<Permission>>>(),
            selector: Arg.Any<Expression<Func<Permission, Permission>>>(), asNoTracking: Arg.Any<bool>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_PermissionsNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new UpdateUserCommand("John", "Doe", "john@test.com", [10UL, 20UL, 30UL]);

        _userRepository
            .GetAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
                include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
                asNoTracking: false,
                cancellationToken: Arg.Any<CancellationToken>()).Returns(new User
                {
                    Id = 1UL,
                    Email = "john@test.com",
                    FirstName = "John",
                    LastName = "Doe"
                });
        _userRepository.ExistsAsync(filter: Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _permissionRepository
            .GetAllAsync(filter: Arg.Any<Expression<Func<Permission, bool>>>(),
                include: Arg.Any<Func<IQueryable<Permission>, IQueryable<Permission>>>(),
                selector: Arg.Any<Expression<Func<Permission, Permission>>>(), asNoTracking: false,
                cancellationToken: Arg.Any<CancellationToken>()).Returns(
            [
                new Permission { Id = 10UL, Name = PermissionName.UserRead },
                new Permission { Id = 20UL, Name = PermissionName.UserCreate }
            ]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await _userRepository.DidNotReceive().UpdateUserPermissionsAsync(user: Arg.Any<User>(),
            permissions: Arg.Any<IEnumerable<Permission>>(), cancellationToken: Arg.Any<CancellationToken>());
    }
}