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

    private async Task PublishImpl<TDomainEvent>(TDomainEvent evet, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
    {
        var eventHandlers = await handlerProvider.GetDomainEventHandler<TDomainEvent>();
        if (!eventHandlers.Any())
            return;

        foreach (var eventHandler in eventHandlers.OrderBy(o => o.Order))
        {
            await eventHandler.HandleEvent(evet, cancellationToken);
        }
    }

    public async Task Publish(IDomainEvent evet, CancellationToken cancellationToken)
    {
        var retTask = GetType()
            .GetMethod(nameof(PublishImpl), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .MakeGenericMethod(evet.GetType())
            .Invoke(this, new object[] { evet, cancellationToken });

        if (retTask is Task task)
            await task;
    }
}