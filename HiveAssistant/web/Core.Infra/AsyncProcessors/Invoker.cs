using Core.App.Extensions;
using Core.App.Handlers;
using Core.Contract;
using Core.Domain.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infra.AsyncProcessors;

internal interface IInvoker
{
    Task RunHandler(string handlerType, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

internal class CommandHandlerInvoker<TCommand>(TCommand command) : IInvoker
    where TCommand : ICommand
{
    public async Task RunHandler(string handlerType, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var type = Type.GetType(handlerType.GetSlimName());
        if (type is null)
            throw new NotSupportedException($"Handler type: {handlerType} not found");

        var handler = (ICommandAsyncHandler<TCommand>)ActivatorUtilities.CreateInstance(serviceProvider, type);
        await handler.HandleCommand(command, cancellationToken);
    }
}

internal class DomainEventHandlerInvoker<TDomainEvent>(TDomainEvent command) : IInvoker
    where TDomainEvent : IDomainEvent
{
    public async Task RunHandler(string handlerType, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var type = Type.GetType(handlerType.GetSlimName());
        if (type is null)
            throw new NotSupportedException($"Handler type: {handlerType} not found");

        var handler = (IDomainEventAsyncHandler<TDomainEvent>)ActivatorUtilities.CreateInstance(serviceProvider, type);
        await handler.HandleEvent(command, cancellationToken);
    }
}