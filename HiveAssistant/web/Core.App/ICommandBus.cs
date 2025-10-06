using Core.Contract;
using Core.Domain.DomainEvents;

namespace Core.App;

public interface ICommandBus
{
    Task ExecuteCommand<TCommand>(TCommand cmd, CancellationToken cancellationToken)
            where TCommand : ICommand;

    Task Publish<TDomainEvent>(TDomainEvent evet, CancellationToken cancellationToken)
            where TDomainEvent : IDomainEvent;
}