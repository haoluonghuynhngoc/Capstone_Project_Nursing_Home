using Mapster;
using Microsoft.EntityFrameworkCore;
using Nursing_Home.Application.Common.Pages;
using Nursing_Home.Application.Repositories;
using Nursing_Home.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Nursing_Home.Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _DbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _DbSet = _context.Set<T>();
    }

    public async Task<bool> ExistsByAsync(
    Expression<Func<T, bool>>? expression = null,
    CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _DbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }
        return await query.AnyAsync(cancellationToken);
    }
    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _DbSet.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(T entity)
    {
        _DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<PaginatedList<TDTO>> FindPaginAsync<TDTO>(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default) where TDTO : class
    {
        IQueryable<T> query = _context.Set<T>();

        if (expression != null) query = query.Where(expression);

        if (orderBy != null) query = orderBy(query);

        return await query.ProjectToType<TDTO>().PaginatedListAsync(pageIndex, pageSize, cancellationToken);
    }

    public async Task<TDTO?> FindByAsyncMapster<TDTO>(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default) where TDTO : class
    {
        IQueryable<T> query = _DbSet;
        query = query.Where(expression);
        return await query.ProjectToType<TDTO>().FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<TDTO>> FindByConditionAsync<TDTO>(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellation = default) where TDTO : class
    {
        IQueryable<T> query = _DbSet;
        if (expression != null)
        {
            query = query.Where(expression);
        }
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ProjectToType<TDTO>().ToListAsync(cancellation);
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _DbSet.FindAsync(id, cancellationToken);
    }

}