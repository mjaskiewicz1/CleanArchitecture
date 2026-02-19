using System.Linq.Expressions;

using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Queries;

using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Queries;

public sealed class GetCurrentUserQueryHandlerTests()
{
    private IUserContext _userContext = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private GetCurrentUserQueryHandler _handler = null!;

    private static User CreateUser(ulong userId = 123ul)
    {
        var permission1 = new Permission { Name = PermissionName.UserCreate };
        permission1.SetId(1);

        var permission2 = new Permission { Name = PermissionName.UserDelete };
        permission2.SetId(2);

        var user = new User
        {
            FirstName = "Admin", LastName = "User", Email = "admin@admin.com", LastLogin = DateTimeOffset.UtcNow
        };
        user.SetId(userId);

        user.UserPermissions = new List<UserPermission>
        {
            new() { UserId = userId, PermissionId = permission1.Id, Permission = permission1 },
            new() { UserId = userId, PermissionId = permission2.Id, Permission = permission2 }
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
        const ulong userId = 123ul;
        var query = new GetCurrentUserQuery();

        _userContext.UserId.Returns(userId);

        var user = CreateUser();

        _userRepository.GetByIdAsync(id: userId, filter: Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<Func<IQueryable<User>, IQueryable<User>>>(), Arg.Any<Expression<Func<User, User>>>(),
                asNoTracking: true,
                cancellationToken: CancellationToken.None)
            .Returns(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();

        await Assert.That(result.Value!.Id).IsEqualTo(user.Id);
        await Assert.That(result.Value.FirstName).IsEqualTo(user.FirstName);
        await Assert.That(result.Value.LastName).IsEqualTo(user.LastName);
        await Assert.That(result.Value.Email).IsEqualTo(user.Email);
        await Assert.That(result.Value.Permissions.Count).IsEqualTo(user.UserPermissions.Count);
    }
}