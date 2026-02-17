namespace Application.Users.Dtos;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);