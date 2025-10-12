using Core.Domain.DomainEvents;

namespace Core.App.Handlers;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    int Order { get; }

    ValueTask HandleEvent(TDomainEvent e, CancellationToken cancellationToken);
}