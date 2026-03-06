using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Errors;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    IUserContext userContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDetailsResponse>>
{
    public async Task<Result<UserDetailsResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(userContext.UserId, cancellationToken);
        return user is null
            ? Result<UserDetailsResponse>.Failure(UserErrors.NotFound)
            : Result.Success(UserDetailsResponse.FromEntity(user));
    }
}