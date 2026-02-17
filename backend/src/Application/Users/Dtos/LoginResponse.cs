namespace Application.Users.Dtos;

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);