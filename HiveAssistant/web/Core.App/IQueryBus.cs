using Core.Contract;

namespace Core.App;

public interface IQueryBus
{
    Task<TResult?> GetQueryResult<TQuery, TResult>(TQuery cmd, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>;
}