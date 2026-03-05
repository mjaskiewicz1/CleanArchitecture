using Application.Abstractions.Data;

using Domain.Repositories;

using Infrastructure.Repositories;

namespace Infrastructure.Database;

public sealed class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        UserRepository = new UserRepository(_context);
        RefreshTokenRepository = new RefreshTokenRepository(_context);
        PermissionRepository = new PermissionRepository(_context);
    }

    private readonly ApplicationDbContext _context;
    public IUserRepository UserRepository { get; }
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    public IPermissionRepository PermissionRepository { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Asynchronously disposes the UnitOfWork and its associated ApplicationDbContext.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
        => _context.DisposeAsync();
}