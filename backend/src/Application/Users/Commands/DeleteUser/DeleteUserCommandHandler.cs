using Application.Abstractions.Data;

using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.UserRepository.ExecuteDeleteAsync(x => x.Id == request.Id, cancellationToken);
        return Result.Success();
    }
}