using Core.Domain.DomainEvents;

namespace Core.Domain.Aggregates;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearAllDomainEvents();
}

public interface IAggregateRoot<TId> : IEntity<TId>, IAggregateRoot
{
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
{
    private readonly Queue<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected void PublishEvent(IDomainEvent<TId> domainEvent)
    {
        _domainEvents.Enqueue(domainEvent);
    }

    public void ClearAllDomainEvents()
    {
        _domainEvents.Clear();
    }
}