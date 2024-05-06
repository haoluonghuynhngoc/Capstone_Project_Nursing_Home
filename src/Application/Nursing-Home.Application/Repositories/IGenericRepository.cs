using Nursing_Home.Application.Common.Pages;
using System.Linq.Expressions;

namespace Nursing_Home.Application.Repositories;
public interface IGenericRepository<T> where T : class
{
    Task<bool> ExistsByAsync(
    Expression<Func<T, bool>>? expression = null,
    CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity);
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TDTO?> FindByAsyncMapster<TDTO>(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default) where TDTO : class;
    Task<IList<TDTO>> FindByConditionAsync<TDTO>(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellation = default) where TDTO : class;
    Task<PaginatedList<TDTO>> FindPaginAsync<TDTO>(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default) where TDTO : class;
}
