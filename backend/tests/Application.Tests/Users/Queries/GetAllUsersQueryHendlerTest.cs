using Application.Abstractions.Data;
using Application.Users.Queries.GetAllUser;

using Domain.Entities;
using Domain.Repositories;

using NSubstitute;

namespace Application.Tests.Users.Queries;

internal sealed class GetAllUsersQueryHandlerTests
{
    private IUnitOfWork _unitOfWork = null!;
    private IUserRepository _userRepository = null!;
    private GetAllUsersQueryHandler _handler = null!;

    private static User CreateUser(ulong id, string email)
        => new()
        {
            Id = id,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            LastLogin = DateTimeOffset.UtcNow,
            UserPermissions = []
        };

    [Before(Test)]
    public void Setup()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();

        _unitOfWork.UserRepository.Returns(_userRepository);

        _handler = new GetAllUsersQueryHandler(_unitOfWork);
    }

    [Test]
    public async Task Handle_WhenRepositoryReturnsUsers_ShouldReturnMappedUsers()
    {
        // Arrange
        var usersFromRepo = new List<User> { CreateUser(1, "a@test.com"), CreateUser(2, "b@test.com") };
        var query = new GetAllUsersQuery { Cursor = 0, TakeAmount = 10 };

        _userRepository.GetUsersWithFiltersAsync(Arg.Is(query.Cursor), Arg.Is(query.TakeAmount), Arg.Any<ulong?>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(usersFromRepo);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();

        var value = result.Value.ToList();
        await Assert.That(value.Count).IsEqualTo(usersFromRepo.Count);
        await Assert.That(value.First().Email).IsEqualTo("a@test.com");
    }

    [Test]
    public async Task Handle_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyCollection()
    {
        // Arrange
        var query = new GetAllUsersQuery { Cursor = 0, TakeAmount = 10 };

        _userRepository.GetUsersWithFiltersAsync(0, 10, null, null, Arg.Any<CancellationToken>()).Returns([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEmpty();
    }
}