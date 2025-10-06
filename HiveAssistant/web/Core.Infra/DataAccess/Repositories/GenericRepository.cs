using System.Linq.Expressions;
using Core.App.Expressions;
using Core.App.Repositories;
using Core.App.Repositories.Include;
using Core.Contract.Queries.Pagination;
using Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Core.Infra.DataAccess.Repositories;

public class GenericRepository<T, TDbContext>(TDbContext dbContext) : IGenericRepository<T>
    where T : class
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext = dbContext;
    protected DbSet<T> DbSet => _dbContext.Set<T>();

    public async ValueTask<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    public async Task<IList<T>> GetAsync(ISpecification<T> specification, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(specification, false);

        query = ApplyOrdering(query, specification);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IList<T>> GetAsync(ISpecification<T> specification, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(specification, false);
        query = includes.Aggregate(query, (current, include) => current.ApplyInclude(include));

        query = ApplyOrdering(query, specification);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IList<TDto>> GetAsync<TDto>(IMapSpecification<T, TDto> specification, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(specification, true);

        query = ApplyOrdering(query, specification);

        return await query.Select(specification.Selector).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<PageResult<T>> GetPagedAsync(IPagedSpecification<T> specification, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(specification, true);
        long? total = null;
        if (specification.Take.HasValue || specification.Skip.HasValue)
        {
            total = await query.CountAsync(cancellationToken);
        }
        query = ApplyOrderingAndPaging(query, specification, specification.Skip, specification.Take);

        var items = await query.ToListAsync(cancellationToken);

        return new PageResult<T>(items, total ?? items.Count, specification.Skip ?? 0, specification.Take);
    }

    public async Task<PageResult<T>> GetPagedAsync(IPagedSpecification<T> specification, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(specification, false);
        query = includes.Aggregate(query, (current, include) => current.ApplyInclude(include));

        long? total = null;
        if (specification.Take.HasValue || specification.Skip.HasValue)
        {
            total = await query.CountAsync(cancellationToken);
        }
        query = ApplyOrderingAndPaging(query, specification, specification.Skip, specification.Take);

        var items = await query.ToListAsync(cancellationToken);

        return new PageResult<T>(items, total ?? items.Count, specification.Skip ?? 0, specification.Take);
    }

    public async Task<PageResult<TDto>> GetPagedAsync<TDto>(IPagedSpecification<T, TDto> specification, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(specification, true);

        long? total = null;
        if (specification.Take.HasValue || specification.Skip.HasValue)
        {
            total = await query.CountAsync(cancellationToken);
        }
        query = ApplyOrderingAndPaging(query, specification, specification.Skip, specification.Take);

        var items = await query.Select(specification.Selector).ToListAsync(cancellationToken: cancellationToken);

        return new PageResult<TDto>(items, total ?? items.Count, specification.Skip ?? 0, specification.Take);
    }

    #region Private

    private IQueryable<T> ApplyFilters(ISpecification<T> specification, bool asNoTracking)
    {
        var query = DbSet.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();

        var filters = specification.AsEnumerableFilters();
        if (filters is not null)
        {
            query = query.Where(filters.Select(x => x.Filter).AndAlso());
        }

        return query;
    }

    private IQueryable<T> ApplyOrdering(IQueryable<T> query, ISpecification<T> specification)
    {
        var orderBy = specification.OrderBy();
        if (orderBy is not null)
        {
            query = orderBy.ToOrderExpression(query);
        }

        return query;
    }

    private IQueryable<T> ApplyOrderingAndPaging(IQueryable<T> query, ISpecification<T> specification, int? skip, int? take)
    {
        var orderBy = specification.OrderBy();
        if (orderBy is not null)
        {
            query = orderBy.ToOrderExpression(query);
        }
        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }
        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return query;
    }

    #endregion Private
}

public class GenericRepository<T, TKey, TDbContext> : GenericRepository<T, TDbContext>, IGenericRepository<T, TKey>
    where T : class, IEntity<TKey>
    where TDbContext : DbContext
{
    public GenericRepository(TDbContext dbContext) : base(dbContext)
    {
    }

    public ValueTask<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
    {
        return DbSet.FindAsync([id], cancellationToken);
    }

    public async Task<T?> GetByIdAsync(TKey id, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken)
    {
        IQueryable<T> query = includes.Aggregate<IncludeTreeExpression<T>, IQueryable<T>>(DbSet, (current, include) => current.ApplyInclude(include));
        return await query
            .Where(x => x.Id!.Equals(id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TDto?> GetByIdAsync<TDto>(TKey id, Expression<Func<T, TDto>> mapExpression, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(x => x.Id!.Equals(id))
            .Select(mapExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }
}