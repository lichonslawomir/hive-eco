using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;

namespace Core.App.Repositories;

public sealed class Specification<TEntity, TKey> : ISpecification<TEntity>
    where TEntity : class
{
    private readonly IOrder<TEntity>? _order;
    private readonly IFilter<TEntity>[] _filters;

    public Specification(IOrder<TEntity> order, params IFilter<TEntity>[] filters)
    {
        _order = order;
        _filters = filters;
        Skip = null;
        Take = null;
    }

    public Specification(params IFilter<TEntity>[] filters)
    {
        _order = null;
        _filters = filters;
        Skip = null;
        Take = null;
    }

    public IEnumerable<IFilter<TEntity>> AsEnumerableFilters()
    {
        return _filters;
    }

    public IOrder<TEntity>? OrderBy()
    {
        return _order;
    }

    public int? Skip { get; set; }
    public int? Take { get; set; }
}