using Application.Common;

using Domain.Entities;

using JetBrains.Annotations;

namespace Application.Users.Dtos;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed record UserResponse : BaseResponse, IResponse<User, UserResponse>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public DateTime? LastLoginUtc { get; init; }

    public static UserResponse FromEntity(User entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            LastLoginUtc = entity.LastLoginUtc,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
}