using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries;

public sealed class GetCurrentUserQueryHandler(
    IUserContext userContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetCurrentUserQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(id: userContext.UserId, include: x => x
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission), asNoTracking: true, cancellationToken: cancellationToken);
        return user is null ? Result<UserResponse>.Failure(Error.NotFound("User not found")) : Result.Success(UserResponse.FromEntity(user));
    }
}