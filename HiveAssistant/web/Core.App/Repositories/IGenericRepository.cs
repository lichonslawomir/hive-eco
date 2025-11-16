using System.Linq.Expressions;
using Core.App.Repositories.Include;
using Core.Contract.Queries.Pagination;
using Core.Domain.Aggregates;

namespace Core.App.Repositories;

public interface IGenericRepository<T>
    where T : class
{
    ValueTask<T> AddAsync(T entity, CancellationToken cancellationToken);

    Task<IList<T>> GetAsync(ISpecification<T> specification, CancellationToken cancellationToken);

    Task<IList<T>> GetAsync(ISpecification<T> specification, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken);

    Task<IList<TDto>> GetAsync<TDto>(IMapSpecification<T, TDto> specification, CancellationToken cancellationToken);

    Task<PageResult<T>> GetPagedAsync(IPagedSpecification<T> specification, CancellationToken cancellationToken);

    Task<PageResult<T>> GetPagedAsync(IPagedSpecification<T> specification, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken);

    Task<PageResult<TDto>> GetPagedAsync<TDto>(IPagedSpecification<T, TDto> specification, CancellationToken cancellationToken);
}

public interface IGenericRepository<T, TKey> : IGenericRepository<T>
    where T : class, IEntity<TKey>
{
    ValueTask<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken);

    Task<TDto?> GetByIdAsync<TDto>(TKey id, Expression<Func<T, TDto>> mapExpression, CancellationToken cancellationToken);

    Task<T?> GetByIdAsync(TKey id, IncludeTreeExpression<T>[] includes, CancellationToken cancellationToken);
}

public static class GenericRepositoryExtensions
{
    public static Task<PageResult<TDto>> GetPagedAsync<T, TDto>(this IGenericRepository<T> repo,
        IMapSpecification<T, TDto> specification,
        int? skip,
        int? take,
        CancellationToken cancellationToken)
        where T : class
    {
        var s = new PagedSpecificationWrapper<T, TDto>(specification)
        {
            Skip = skip,
            Take = take
        };
        return repo.GetPagedAsync(s, cancellationToken);
    }
}