using Domain.Entities;
using Domain.Repositories;

using Infrastructure.Database;
using Infrastructure.Repositories.Extensions;
using Infrastructure.Repositories.Generic;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Include(x => x.UserPermissions)
            .ThenInclude(up => up.Permission)
            .Select(u => new User
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                LastLogin = u.LastLogin,
                UserPermissions = u.UserPermissions
                    .Select(up => new UserPermission
                    {
                        PermissionId = up.PermissionId,
                        Permission = new Permission { Id = up.Permission.Id, Name = up.Permission.Name }
                    })
                    .ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return user;
    }

    public async Task<List<User>> GetUsersWithFiltersAsync(ulong cursor, uint take, ulong? id,
        string? email = null, CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .ApplyFilter(id, user => user.Id == id)
            .ApplyFilter(email, user => EF.Functions.Like(user.Email, $"%{email}%"))
            .ApplyCursorPagination(cursor, take)
            .Select(u => new User
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                LastLogin = u.LastLogin,
                UserPermissions = u.UserPermissions
                    .Select(up => new UserPermission
                    {
                        PermissionId = up.PermissionId,
                        Permission = new Permission { Id = up.Permission.Id, Name = up.Permission.Name }
                    }).ToList()
            })
            .ToListAsync(cancellationToken);
        return users;
    }

    public async Task<User> UpdateUserPermissionsAsync(User user, List<Permission> permissions,
        CancellationToken cancellationToken)
    {
        var permissionIds = permissions.ConvertAll(p => p.Id);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingIds = user.UserPermissions.Select(up => up.PermissionId).ToHashSet();
            foreach (var pid in permissionIds.Where(pid => !existingIds.Contains(pid)))
            {
                user.UserPermissions.Add(new UserPermission { UserId = user.Id, PermissionId = pid });
            }

            await _context.SaveChangesAsync(cancellationToken);

            var removeIds = user.UserPermissions
                .Select(up => up.PermissionId)
                .Where(id => !permissionIds.Contains(id))
                .ToList();

            if (removeIds.Count != 0)
            {
                await _context.UserPermissions
                    .Where(up => up.UserId == user.Id && removeIds.Contains(up.PermissionId))
                    .ExecuteDeleteAsync(cancellationToken);

                user.UserPermissions.RemoveAll(up => removeIds.Contains(up.PermissionId));
            }

            await transaction.CommitAsync(cancellationToken);

            return user;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}