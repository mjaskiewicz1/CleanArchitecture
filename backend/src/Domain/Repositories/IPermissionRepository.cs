using System.Collections.Immutable;

using Domain.Entities;
using Domain.Repositories.Generic;

namespace Domain.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    public Task<bool> AllExistAsync(ImmutableHashSet<ulong> permissionIds, CancellationToken cancellationToken = default);
}