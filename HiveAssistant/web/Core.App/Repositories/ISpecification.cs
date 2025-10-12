using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;

namespace Core.App.Repositories;

public interface ISpecification<TEntity>
    where TEntity : class
{
    IEnumerable<IFilter<TEntity>> AsEnumerableFilters();

    IOrder<TEntity>? OrderBy();
}