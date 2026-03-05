using System.Collections.Immutable;

using Application.Common;
using Application.Users.Dtos;

using Domain.Entities;
using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(string FirstName, string LastName, string Email, ImmutableHashSet<ulong> PermissionIds)
    : ICreateRequest<User>, IRequest<Result<UserResponse>>
{
    public User ToEntity()
    {
        var user = new User { FirstName = FirstName, LastName = LastName, Email = Email };
        foreach (var permissionId in PermissionIds)
        {
            user.UserPermissions.Add(new UserPermission { PermissionId = permissionId });
        }

        return user;
    }
}