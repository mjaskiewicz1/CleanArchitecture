using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Errors;
using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserByIdQuery, Result<UserDetailsResponse>>
{
    public async Task<Result<UserDetailsResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(request.Id, cancellationToken);
        return user is null
            ? Result<UserDetailsResponse>.Failure(UserErrors.NotFound)
            : Result.Success(UserDetailsResponse.FromEntity(user));
    }
}