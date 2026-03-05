using System.Collections.Immutable;

using Application.Common;
using Application.Users.Dtos;

using Domain.Entities;
using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(string FirstName, string LastName, string Email, ImmutableHashSet<ulong> PermissionIds)
    : UpdateRequest<User>, IRequest<Result<UserDetailsResponse>>
{
    public override void Update(User entity)
    {
        entity.FirstName = FirstName;
        entity.LastName = LastName;
        entity.Email = Email;
    }
}