namespace Core.Domain.DomainEvents;

public interface IDomainEvent
{
}

public interface IDomainEvent<out TId> : IDomainEvent
{
    TId EntityId { get; }
}