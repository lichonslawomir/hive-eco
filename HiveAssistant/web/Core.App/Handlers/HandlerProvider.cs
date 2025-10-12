using Core.App.Extensions;
using Core.App.Handlers.Async;
using Core.Contract;
using Core.Domain.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Core.App.Handlers;

public interface IHandlerProvider
{
    ValueTask<IQueryHandler<TQuery, TResult>> GetQueryHandler<TQuery, TResult>() where TQuery : IQuery<TResult>;

    ValueTask<ICommandHandler<TCommand>[]> GetCommandHandler<TCommand>() where TCommand : ICommand;

    ValueTask<IDomainEventHandler<TDomainEvent>[]> GetDomainEventHandler<TDomainEvent>() where TDomainEvent : IDomainEvent;

    TService GetRequiredService<TService>() where TService : notnull;

    void Reset();
}

internal sealed class HandlerProvider : IHandlerProvider, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AssemblyScanner _assemblyScanner;

    private readonly IDictionary<Type, object> _scopeHandlers = new Dictionary<Type, object>();

    public HandlerProvider(IServiceProvider serviceProvider, AssemblyScanner assemblyScanner)
    {
        _serviceProvider = serviceProvider;
        _assemblyScanner = assemblyScanner;
    }

    public void Dispose()
    {
        _scopeHandlers.Clear();
    }

    public void Reset()
    {
        _scopeHandlers.Clear();
    }

    private object Create(Type type, params Type[] genericParams)
    {
        //This is scope, so we don't need any critical section
        if (type.IsOpenGeneric())
        {
            var closedType = type.MakeGenericType(genericParams);
            if (_scopeHandlers.TryGetValue(closedType, out var obj))
                return obj;
            obj = ActivatorUtilities.CreateInstance(_serviceProvider, closedType);
            _scopeHandlers[type] = obj;
            return obj;
        }
        else
        {
            if (_scopeHandlers.TryGetValue(type, out var obj))
                return obj;
            obj = ActivatorUtilities.CreateInstance(_serviceProvider, type);
            _scopeHandlers[type] = obj;
            return obj;
        }
    }

    public async ValueTask<ICommandHandler<TCommand>[]> GetCommandHandler<TCommand>() where TCommand : ICommand
    {
        var list = await _assemblyScanner.Resolve<ICommandHandler<TCommand>>();
        var list2 = await _assemblyScanner.Resolve<ICommandAsyncHandler<TCommand>>();
        return list.Select(type => (ICommandHandler<TCommand>)Create(type, typeof(TCommand)))
            .Union(
                list2.Select(asyncType =>
                    (ICommandHandler<TCommand>)Create(typeof(AsyncCommandHandlerWrapperr<,>), asyncType, typeof(TCommand))
                )
            ).ToArray();
    }

    public async ValueTask<IDomainEventHandler<TDomainEvent>[]> GetDomainEventHandler<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        var list = await _assemblyScanner.Resolve<IDomainEventHandler<TDomainEvent>>();
        var list2 = await _assemblyScanner.Resolve<IDomainEventAsyncHandler<TDomainEvent>>();
        return list.Select(type => (IDomainEventHandler<TDomainEvent>)Create(type, typeof(TDomainEvent)))
            .Union(
                list2.Select(asyncType =>
                    (IDomainEventHandler<TDomainEvent>)Create(typeof(AsyncDomainEventHandlerWrapperr<,>), asyncType, typeof(TDomainEvent))
                )
            ).ToArray();
    }

    public async ValueTask<IQueryHandler<TQuery, TResult>> GetQueryHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
    {
        var list = await _assemblyScanner.Resolve<IQueryHandler<TQuery, TResult>>();
        return list.Select(type => (IQueryHandler<TQuery, TResult>)Create(type)).Last();
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        return _serviceProvider.GetRequiredService<TService>();
    }
}