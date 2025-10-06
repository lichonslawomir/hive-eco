using Core.App.Handlers;
using Core.Contract;
using Core.Domain.DomainEvents;

namespace Core.App.Bus;

internal class CommandBus(IHandlerProvider handlerProvider) : ICommandBus
{
    public async Task ExecuteCommand<TCommand>(TCommand cmd, CancellationToken cancellationToken) where TCommand : ICommand
    {
        var cmdHandlers = await handlerProvider.GetCommandHandler<TCommand>();
        if (!cmdHandlers.Any())
            throw new NotImplementedException($"Handler not found: {typeof(TCommand).Name} ({typeof(TCommand).FullName})");

        foreach (var cmdHandler in cmdHandlers.OrderBy(o => o.Order))
        {
            await cmdHandler.HandleCommand(cmd, cancellationToken);
        }
    }

    public async Task Publish<TDomainEvent>(TDomainEvent evet, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
    {
        var eventHandlers = await handlerProvider.GetDomainEventHandler<TDomainEvent>();
        if (!eventHandlers.Any())
            return;

        foreach (var eventHandler in eventHandlers.OrderBy(o => o.Order))
        {
            await eventHandler.HandleEvent(evet, cancellationToken);
        }
    }
}