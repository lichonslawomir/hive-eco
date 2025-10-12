using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace Core.App.Repositories;

public interface IPagedSpecification<TEntity> : ISpecification<TEntity>
    where TEntity : class
{
    int? Skip { get; set; }
    int? Take { get; set; }
}

public interface IPagedSpecification<TEntity, TDto> : IMapSpecification<TEntity, TDto>
    where TEntity : class
{
    int? Skip { get; set; }
    int? Take { get; set; }
}

public sealed class PagedSpecificationWrapper<TEntity>(ISpecification<TEntity> baseSpec) : IPagedSpecification<TEntity>
    where TEntity : class
{
    public int? Skip { get; set; }
    public int? Take { get; set; }

    public IEnumerable<IFilter<TEntity>>? AsEnumerableFilters()
    {
        return baseSpec.AsEnumerableFilters();
    }

    public IOrder<TEntity>? OrderBy()
    {
        return baseSpec.OrderBy();
    }
}

public sealed class PagedSpecificationWrapper<TEntity, TDto>(IMapSpecification<TEntity, TDto> baseSpec) : IPagedSpecification<TEntity, TDto>
    where TEntity : class
{
    public int? Skip { get; set; }
    public int? Take { get; set; }

    public IEnumerable<IFilter<TEntity>>? AsEnumerableFilters()
    {
        return baseSpec.AsEnumerableFilters();
    }

    public IOrder<TEntity>? OrderBy()
    {
        return baseSpec.OrderBy();
    }

    public bool Distinct => baseSpec.Distinct;

    public Expression<Func<TEntity, TDto>> Selector()
    {
        return baseSpec.Selector();
    }
}