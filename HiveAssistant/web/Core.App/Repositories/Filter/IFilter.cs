using System.Linq.Expressions;

namespace Core.App.Repositories.Filter;

public interface IFilter<T> where T : class
{
    Expression<Func<T, bool>> Filter { get; }
}