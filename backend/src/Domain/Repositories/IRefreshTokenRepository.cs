using Domain.Entities;
using Domain.Repositories.Generic;

namespace Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    public Task RotateRefreshTokenAsync(ulong id, string newToken, DateTime newExpirationDate,
        CancellationToken cancellationToken = default);
}