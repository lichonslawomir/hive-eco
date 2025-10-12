using Core.App.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.App.Handlers;

internal sealed class AssemblyScanner
{
    private readonly IServiceProvider _serviceProvider;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly IDictionary<Type, IList<Type>> _handlers = new Dictionary<Type, IList<Type>>();

    public AssemblyScanner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private IEnumerable<Type> Resolve(Type handlerType)
    {
        foreach (var type in _serviceProvider.GetServices<IHandlerAssembly>().SelectMany(a => a.GetAssembly().GetTypes()))
        {
            if (!type.IsConcrete())
                continue;
            if (type.HasImplemented(handlerType))
                yield return type;
        }
    }

    public async ValueTask<IList<Type>> Resolve<THandler>()
    {
        var handlerType = typeof(THandler);
        if (_handlers.TryGetValue(handlerType, out var list))
            return list;

        //This is singleton, so we need to have critical section among many tasks
        await _semaphore.WaitAsync();
        try
        {
            if (_handlers.TryGetValue(handlerType, out list))
                return list;//Already register

            list = Resolve(handlerType).ToList();
            _handlers[handlerType] = list;
            return list;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}