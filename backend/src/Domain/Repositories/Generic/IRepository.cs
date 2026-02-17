using System.Linq.Expressions;

using Domain.Entities.Generic;

namespace Domain.Repositories.Generic;

/// <summary>
/// Repository pattern implementation inspired by:
/// <see href="https://medium.com/@codebob75/repository-pattern-c-ultimate-guide-entity-framework-core-clean-architecture-dtos-dependency-6a8d8b444dcb" />
/// </summary>
public interface IRepository<TEntity> where TEntity : DbEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(uint cursor, uint take,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(ulong id, Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    Task<int> ExecuteDeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);
}