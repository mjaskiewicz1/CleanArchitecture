using System.Net;

using Application.Abstractions.Data;
using Application.Users.Queries.GetUserById;

using Domain.Entities;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Queries;

public sealed class GetUserByIdQueryHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private GetUserByIdQueryHandler _handler = null!;

    private static User CreateUser(ulong userId = 123ul)
    {
        var user = new User
        {
            Id = userId,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@admin.com",
            LastLogin = DateTimeOffset.UtcNow,
            UserPermissions = []
        };


        return user;
    }


    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();

        _unitOfWork.UserRepository.Returns(_userRepository);

        _handler = new GetUserByIdQueryHandler(_unitOfWork);
    }

    [Test]
    public async Task Handle_GetCurrentUserQuery_ReturnsCurrentUser()
    {
        // Arrange
        var user = CreateUser();
        var query = new GetUserByIdQuery(user.Id);

        _userRepository.GetUserByIdAsync(user.Id, CancellationToken.None).Returns(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).GetUserByIdAsync(user.Id, CancellationToken.None);
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
        var query = new GetUserByIdQuery(999ul);

        _userRepository.GetUserByIdAsync(999ul, CancellationToken.None).Returns((User)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}