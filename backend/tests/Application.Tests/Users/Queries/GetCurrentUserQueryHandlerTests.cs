using System.Linq.Expressions;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Queries;

using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Queries;

public sealed class GetCurrentUserQueryHandlerTests
{
    private IUserContext _userContext = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private GetCurrentUserQueryHandler _handler = null!;

    private static User CreateUser(ulong userId = 123ul)
    {
        var permission1 = new Permission { Id = 1, Name = PermissionName.UserCreate };

        var permission2 = new Permission { Id = 2, Name = PermissionName.UserDelete };

        var user = new User
        {
            Id = userId,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@admin.com",
            LastLogin = DateTimeOffset.UtcNow,
            UserPermissions =
            [
                new() { UserId = userId, PermissionId = permission1.Id, Permission = permission1 },
                new() { UserId = userId, PermissionId = permission2.Id, Permission = permission2 }
            ]
        };


        return user;
    }


    [Before(Test)]
    public void Setup()
    {
        _userContext = Substitute.For<IUserContext>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();

        _unitOfWork.UserRepository.Returns(_userRepository);

        _handler = new GetCurrentUserQueryHandler(_userContext, _unitOfWork);
    }

    [Test]
    public async Task Handle_GetCurrentUserQuery_ReturnsCurrentUser()
    {
        // Arrange
        var query = new GetCurrentUserQuery();
        var user = CreateUser();

        _userContext.UserId.Returns(user.Id);

        _userRepository.GetByIdAsync(
                id: user.Id,
                filter: Arg.Any<Expression<Func<User, bool>>>(),
                include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
                selector: Arg.Any<Expression<Func<User, User>>>(),
                asNoTracking: Arg.Any<bool>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Id).IsEqualTo(user.Id);
        await Assert.That(result.Value.FirstName).IsEqualTo(user.FirstName);
        await Assert.That(result.Value.LastName).IsEqualTo(user.LastName);
        await Assert.That(result.Value.Email).IsEqualTo(user.Email);
        await Assert.That(result.Value.Permissions.Count).IsEqualTo(user.UserPermissions.Count);
    }

    [Test]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var query = new GetCurrentUserQuery();

        _userContext.UserId.Returns(999ul);

        _userRepository.GetByIdAsync(
                id: Arg.Any<ulong>(),
                filter: Arg.Any<Expression<Func<User, bool>>>(),
                include: Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(),
                selector: Arg.Any<Expression<Func<User, User>>>(),
                asNoTracking: Arg.Any<bool>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();

    }
}