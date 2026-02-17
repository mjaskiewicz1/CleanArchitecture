using Domain.Entities;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
    public bool ValidateRefreshToken(string token, string tokenHash);
}