using Core.App.Handlers;
using Core.Contract;

namespace Core.App.Bus;

internal class QueryBus(IHandlerProvider handlerProvider) : IQueryBus
{
    public async Task<TResult?> GetQueryResult<TQuery, TResult>(TQuery cmd, CancellationToken cancellationToken) where TQuery : IQuery<TResult>
    {
        var handler = await handlerProvider.GetQueryHandler<TQuery, TResult>();
        return await handler.HandleQuery(cmd, cancellationToken);
    }
}