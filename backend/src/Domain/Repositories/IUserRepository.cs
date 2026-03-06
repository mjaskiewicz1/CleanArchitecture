using Domain.Entities;
using Domain.Repositories.Generic;

namespace Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetUserByIdAsync(ulong id, CancellationToken cancellationToken = default);

    public Task<List<User>> GetUsersWithFiltersAsync(ulong cursor, uint take, ulong? id,
        string? email = null, CancellationToken cancellationToken = default);

    public Task<User> UpdateUserPermissionsAsync(User user, List<Permission> permissions,
        CancellationToken cancellationToken);

    Task<bool> SetPasswordByTokenAsync(string token, string hashedPassword, CancellationToken cancellationToken);
}