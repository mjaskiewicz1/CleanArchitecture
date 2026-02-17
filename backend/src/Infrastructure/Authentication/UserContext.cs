using Application.Abstractions.Authentication;

using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{

    public ulong UserId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new UnauthorizedAccessException("User is not authenticated.");
}