using Core.Domain.DomainEvents;

namespace Core.App.Handlers;

public interface IDomainEventAsyncHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    int Order { get; }

    ValueTask<string> AsyncTaskId(TDomainEvent e);

    ValueTask HandleEvent(TDomainEvent e, CancellationToken cancellationToken);
}