using System.Collections.Immutable;

using Domain.Entities;
using Domain.Repositories;

using Infrastructure.Database;
using Infrastructure.Repositories.Generic;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PermissionRepository(ApplicationDbContext dbContext)
    : Repository<Permission>(dbContext), IPermissionRepository
{
    public async Task<bool> AllExistAsync(ImmutableHashSet<ulong> permissionIds, CancellationToken cancellationToken = default)
    {
        if (permissionIds.Count == 0)
            return true;

        var count = await _context.Permissions
            .AsNoTracking()
            .CountAsync(p => permissionIds.Contains(p.Id), cancellationToken);

        return count == permissionIds.Count;
    }
}