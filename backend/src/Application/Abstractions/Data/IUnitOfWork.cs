using Domain.Repositories;

namespace Application.Abstractions.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    public IUserRepository UserRepository { get; }
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    public IPermissionRepository PermissionRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}