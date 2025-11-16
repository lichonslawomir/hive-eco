using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Include;
using Core.Domain.Aggregates;

namespace Core.App.Extensions;

public static class GenericRepositoryExtensions
{
    public static async Task<T?> GetFirstOrDefaultAsync<T>(this IGenericRepository<T> repo, ISpecification<T> specification, CancellationToken cancellationToken)
        where T : class
    {
        var pagedSpec = new PagedSpecificationWrapper<T>(specification)
        {
            Take = 1
        };
        var result = await repo.GetPagedAsync(pagedSpec, cancellationToken);
        if (result.Items.Any())
        {
            return result.Items.First();
        }

        return null;
    }

    public static async Task<T?> GetFirstOrDefaultAsync<T>(this IGenericRepository<T> repo, ISpecification<T> specification, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken)
        where T : class
    {
        var pagedSpec = new PagedSpecificationWrapper<T>(specification)
        {
            Take = 1
        };
        var result = await repo.GetPagedAsync(pagedSpec, includes, cancellationToken);
        if (result.Items.Any())
        {
            return result.Items.First();
        }

        return null;
    }

    public static async Task<(TDto? dto, bool exists)> GetFirstOrDefaultAsync<T, TDto>(this IGenericRepository<T> repo, IMapSpecification<T, TDto> specification, CancellationToken cancellationToken)
        where T : class
    {
        var pagedSpec = new PagedSpecificationWrapper<T, TDto>(specification)
        {
            Take = 1
        };
        var result = await repo.GetPagedAsync(pagedSpec, cancellationToken);
        if (result.Items.Any())
        {
            return (result.Items.First(), true);
        }

        return (default, false);
    }

    public static async Task<T> GetSingleAsync<T>(this IGenericRepository<T> repo, ISpecification<T> specification, CancellationToken cancellationToken)
        where T : class
    {
        var pagedSpec = new PagedSpecificationWrapper<T>(specification)
        {
            Take = 2
        };
        var result = await repo.GetPagedAsync(pagedSpec, cancellationToken);
        if (result.Items.Any() && result.Total == 1)
        {
            return result.Items.First();
        }
        if (result.Total > 1)
            throw new InvalidOperationException("More than one element");
        throw new InvalidOperationException("No elements");
    }

    public static async Task<T> GetSingleAsync<T>(this IGenericRepository<T> repo, ISpecification<T> specification, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken)
        where T : class
    {
        var pagedSpec = new PagedSpecificationWrapper<T>(specification)
        {
            Take = 2
        };
        var result = await repo.GetPagedAsync(pagedSpec, includes, cancellationToken);
        if (result.Items.Any() && result.Total == 1)
        {
            return result.Items.Single();
        }
        if (result.Total > 1)
            throw new InvalidOperationException("More than one element");
        throw new InvalidOperationException("No elements");
    }

    public static async Task<IList<T>> GetAsync<T, TKey>(this IGenericRepository<T, TKey> repo, IFilter<T>[] filters, CancellationToken cancellationToken)
        where T : class, IEntity<TKey>
    {
        return await repo.GetAsync(new Specification<T, TKey>(filters), cancellationToken);
    }

    public static async Task<IList<T>> GetAsync<T, TKey>(this IGenericRepository<T, TKey> repo, IFilter<T>[] filters, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken)
        where T : class, IEntity<TKey>
    {
        return await repo.GetAsync(new Specification<T, TKey>(filters), includes, cancellationToken);
    }
}