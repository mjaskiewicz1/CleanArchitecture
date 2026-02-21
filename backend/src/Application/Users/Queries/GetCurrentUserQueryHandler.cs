using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Entities;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries;

public sealed class GetCurrentUserQueryHandler(
    IUserContext userContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetCurrentUserQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(
            id: userContext.UserId,
            selector: u => new User
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                LastLogin = u.LastLogin,
                UserPermissions = u.UserPermissions
                    .Select(up => new UserPermission
                    {
                        PermissionId = up.PermissionId,
                        Permission = new Permission { Id = up.Permission.Id, Name = up.Permission.Name },
                        UserId = u.Id
                    })
                    .ToList()
            },
            asNoTracking: true,
            cancellationToken: cancellationToken
        );
        return user is null
            ? Result<UserProfileResponse>.Failure(Error.NotFound("User not found"))
            : Result.Success(UserProfileResponse.FromEntity(user));
    }
}