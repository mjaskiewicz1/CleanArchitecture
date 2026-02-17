using Domain.Entities;
using Domain.Repositories;

using Infrastructure.Database;
using Infrastructure.Repositories.Generic;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task RotateRefreshTokenAsync(ulong id, string newToken, DateTime newExpirationDate,
        CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens
            .Where(rt => rt.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.Token, newToken)
                .SetProperty(rt => rt.ExpiresAtUtc, newExpirationDate), cancellationToken);
    }
}