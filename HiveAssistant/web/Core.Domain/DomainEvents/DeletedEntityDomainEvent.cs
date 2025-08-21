using Core.Domain.Aggregates;
using System.Text.Json.Serialization;

namespace Core.Domain.DomainEvents;

public interface IDeletedEntityDomainEvent<out TId> : IDomainEvent<TId>
{
}

public abstract class DeletedEntityDomainEvent<TId> : IDeletedEntityDomainEvent<TId>
{
    private readonly TId? _entityId;

    [JsonConstructor]
    public DeletedEntityDomainEvent(TId entityId)
    {
        _entityId = entityId;
    }

    public DeletedEntityDomainEvent(IEntity<TId> entity)
    {
        _entityId = entity.Id;
    }

    public TId EntityId
    {
        get
        {
            return _entityId ?? throw new NotSupportedException("Id not initialized");
        }
    }
}