using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Email;
using Application.Users.Dtos;

using Domain.Errors;
using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenProvider tokenProvider,
    IEmailSender emailSender,
    IEmailContentFactory emailFactory) : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await unitOfWork.UserRepository.ExistsAsync(x => x.Email == request.Email, cancellationToken))
            return Result<UserResponse>.Failure(UserErrors.EmailAlreadyExists);

        if (!await unitOfWork.PermissionRepository.AllExistAsync(request.PermissionIds, cancellationToken))
            return Result<UserResponse>.Failure(UserErrors.PermissionsNotFound);


        var user = request.ToEntity();
        user.PasswordResetToken = tokenProvider.CreatePasswordResetToken();
        user.PasswordResetTokenExpiryUtc = DateTime.UtcNow.AddHours(3);

        await unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);


        (string subject, string body) = emailFactory.CreateWelcomeSetPasswordEmail(user);

        await emailSender.SendEmailAsync(user.Email, subject, body, cancellationToken);

        return Result.Success(UserResponse.FromEntity(user));
    }
}