using System.Linq.Expressions;

using Domain.Entities.Generic;
using Domain.Repositories.Generic;

using Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Generic;

public class Repository<TEntity>(ApplicationDbContext dbContext) : IRepository<TEntity>
    where TEntity : DbEntity
{
    protected readonly ApplicationDbContext _context = dbContext;

    private IQueryable<TEntity> BaseQuery(bool asNoTracking)
        => asNoTracking ? _context.Set<TEntity>().AsNoTracking() : _context.Set<TEntity>();

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = BaseQuery(asNoTracking).OrderBy(x => x.Id);

        if (include is not null)
            query = include(query);

        if (filter is not null)
            query = query.Where(filter);


        if (selector is not null)
            query = query.Select(selector);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(uint cursor, uint take,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = BaseQuery(asNoTracking).OrderBy(x => x.Id);

        if (include is not null)
            query = include(query);

        if (filter is not null)
            query = query.Where(filter);

        query = query.Where(x => x.Id > cursor);


        if (selector is not null)
            query = query.Select(selector);

        return await query.Take((int)take).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null, bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = BaseQuery(asNoTracking);

        if (include is not null)
            query = include(query);

        if (filter is not null)
            query = query.Where(filter);

        if (selector is not null)
            query = query.Select(selector);

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(ulong id, Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = BaseQuery(asNoTracking);

        query = query.Where(x => x.Id == id);

        if (filter is not null)
            query = query.Where(filter);

        if (include is not null)
            query = include(query);

        if (selector is not null)
            query = query.Select(selector);

        return await query.SingleOrDefaultAsync(cancellationToken);
    }


    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);

    public async Task<int> ExecuteDeleteAsync(Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().Where(filter).ExecuteDeleteAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().AsNoTracking().AnyAsync(filter, cancellationToken);

    public async Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
        => filter == null
            ? await _context.Set<TEntity>().CountAsync(cancellationToken)
            : await _context.Set<TEntity>().Where(filter).CountAsync(cancellationToken);
}