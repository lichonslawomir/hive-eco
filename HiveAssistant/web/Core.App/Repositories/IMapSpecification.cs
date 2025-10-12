using System.Linq.Expressions;

namespace Core.App.Repositories;

public interface IMapSpecification<TEntity, TDto>
    : ISpecification<TEntity> where TEntity : class
{
    bool Distinct { get; }

    Expression<Func<TEntity, TDto>> Selector();
}