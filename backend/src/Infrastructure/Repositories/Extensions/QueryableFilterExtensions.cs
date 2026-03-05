using System.Linq.Expressions;

using Domain.Entities.Generic;

namespace Infrastructure.Repositories.Extensions;

public static class QueryableFilterExtensions
{
    extension<T>(IQueryable<T> query) where T : DbEntity
    {
        public IQueryable<T> ApplyCursorPagination(ulong cursor, uint takeAmount) => query
            .OrderBy(x => x.Id)
            .Where(x => x.Id > cursor)
            .Take((int)takeAmount);

        public IQueryable<T> ApplyFilter(ulong? id, Expression<Func<T, bool>> predicate) =>
            id.HasValue ? query.Where(predicate) : query;

        public IQueryable<T> ApplyFilter(string? filterParam, Expression<Func<T, bool>> predicate) =>
            string.IsNullOrWhiteSpace(filterParam) ? query : query.Where(predicate);
    }
}