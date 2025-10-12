using Core.Contract;

namespace Core.App.Handlers;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult?> HandleQuery(TQuery query, CancellationToken cancellationToken);
}