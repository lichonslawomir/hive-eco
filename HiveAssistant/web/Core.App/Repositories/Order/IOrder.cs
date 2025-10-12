using System.Linq.Expressions;

namespace Core.App.Repositories.Order;

public interface IOrder<T> where T : class
{
    IOrderedQueryable<T> ToOrderExpression(IQueryable<T> entities);
}

public abstract class AOrder<TEntity, TKey>(bool asc) : IOrder<TEntity>
    where TEntity : class
{
    public abstract Expression<Func<TEntity, TKey>> OrderFunc { get; }

    public IOrderedQueryable<TEntity> ToOrderExpression(IQueryable<TEntity> entities)
    {
        return asc
            ? entities.OrderBy(OrderFunc)
            : entities.OrderByDescending(OrderFunc);
    }

    public override string ToString()
    {
        return $"Asc:{asc}, OrderFunc: {OrderFunc}";
    }
}