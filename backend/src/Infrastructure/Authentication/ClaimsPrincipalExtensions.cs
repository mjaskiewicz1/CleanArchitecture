using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static ulong GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return ulong.TryParse(userId, out ulong parsedUserId) ?
            parsedUserId :
            throw new UnauthorizedException("User id is unavailable");
    }
}