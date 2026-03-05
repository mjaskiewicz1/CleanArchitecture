using Application.Abstractions.Data;
using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

namespace Application.Users.Queries.GetAllUser;

public sealed class GetAllUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserResponse>>>
{
    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var userFromDb = await unitOfWork.UserRepository.GetUsersWithFiltersAsync(request.Cursor, request.TakeAmount,
            request.Id, request.Email, cancellationToken);
        var users = userFromDb.Select(UserResponse.FromEntity);
        return Result.Success(users);
    }
}