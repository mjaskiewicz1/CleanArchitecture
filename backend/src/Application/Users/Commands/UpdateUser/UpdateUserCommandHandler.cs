using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, Result<UserDetailsResponse>>
{
    public async Task<Result<UserDetailsResponse>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetAsync(asNoTracking: false, filter: x => x.Id == request.Id,
            include: x => x.Include(u => u.UserPermissions), cancellationToken: cancellationToken);

        if (user is null)
            return Result<UserDetailsResponse>.Failure(Error.NotFound("User not found"));

        if (await unitOfWork.UserRepository.ExistsAsync(x => x.Email == request.Email && x.Id != request.Id,
                cancellationToken))
        {
            return Result<UserDetailsResponse>.Failure(Error.Conflict("User with this email already exists"));
        }

        var permissions = await unitOfWork.PermissionRepository.GetAllAsync(asNoTracking: false, filter:
            x => request.PermissionIds.Contains(x.Id), cancellationToken: cancellationToken);

        if (permissions.Count != request.PermissionIds.Count)
            return Result<UserDetailsResponse>.Failure(Error.BadRequest("One or more permissions do not exist"));
        request.Update(user);
        await unitOfWork.UserRepository.UpdateUserPermissionsAsync(user, permissions, cancellationToken);

        return Result.Success(UserDetailsResponse.FromEntity(user));
    }
}